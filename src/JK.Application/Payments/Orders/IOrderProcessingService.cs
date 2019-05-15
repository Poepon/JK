using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Timing;
using JK.Cryptography;
using JK.Dto;
using JK.Payments.Bacis;
using JK.Payments.Enumerates;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Payments.ThirdParty;
using Microsoft.EntityFrameworkCore;

namespace JK.Payments.Orders
{
    public class BuildOrderPostRequestResult
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public RequestType RequestType { get; set; }

        public string ContentType { get; set; }

        public string DataType { get; set; }

        public string AcceptCharset { get; set; }

        public Dictionary<string, string> Parameters { get; set; }
    }
    public interface IOrderProcessingService
    {

        /// <summary>
        /// 提交一个支付请求
        /// </summary>
        Task<ResultDto<PaymentOrderDto>> PlaceOrderAsync(CreatePaymentOrderDto input);

        Task<BuildOrderPostRequestResult> BuildOrderPostRequest(PaymentOrderDto paymentOrder);
    }

    public class OrderProcessingService : AbpServiceBase, IOrderProcessingService
    {
        private readonly ITenantCache tenantCache;
        private readonly IPaymentOrderPolicyService paymentOrderPolicyService;
        private readonly IIdGenerator idGenerator;
        private readonly IRepository<Company> companyRepository;
        private readonly IRepository<Channel> channelRepository;
        private readonly IRepository<Bank> bankRepository;
        private readonly IRepository<ChannelOverride> channelOverrideRepository;
        private readonly IRepository<ThirdPartyAccount> accountRepository;
        private readonly IRepository<PaymentOrder, long> paymentOrderRepository;
        private readonly IRepository<ApiConfiguration> apiconfigRepository;
        private readonly IRepository<ApiParameter> apiParameterRepository;

        public OrderProcessingService(
            ITenantCache tenantCache,
            IPaymentOrderPolicyService paymentOrderPolicyService,
            IIdGenerator idGenerator,
            IRepository<Company> companyRepository,
            IRepository<Channel> channelRepository,
            IRepository<ChannelOverride> channelOverrideRepository,
            IRepository<Bank> bankRepository,
            IRepository<ThirdPartyAccount> accountRepository,
            IRepository<PaymentOrder, long> paymentOrderRepository,
            IRepository<ApiConfiguration> apiconfigRepository,
            IRepository<ApiParameter> apiParameterRepository)
        {
            this.tenantCache = tenantCache;
            this.paymentOrderPolicyService = paymentOrderPolicyService;
            this.idGenerator = idGenerator;
            this.companyRepository = companyRepository;
            this.channelRepository = channelRepository;
            this.channelOverrideRepository = channelOverrideRepository;
            this.bankRepository = bankRepository;
            this.accountRepository = accountRepository;
            this.paymentOrderRepository = paymentOrderRepository;
            this.apiconfigRepository = apiconfigRepository;
            this.apiParameterRepository = apiParameterRepository;
        }

        public async Task<ResultDto<PaymentOrderDto>> PlaceOrderAsync(CreatePaymentOrderDto input)
        {
            var tenant = tenantCache.Get(input.TenantId);

            if (!string.IsNullOrEmpty(input.ExternalOrderId))
            {
                var isDuplicate = CheckDuplicateOrders(input.TenantId, input.ExternalOrderId);
                if (isDuplicate)
                {
                    return new ResultDto<PaymentOrderDto> { IsSuccess = false, ErrorMessage = "商户订单号重复！" };
                }
            }
            //支付通道
            var channel = await channelRepository.FirstOrDefaultAsync(c => c.ChannelCode == input.ChannelCode);
            int channelId = channel.Id;
            //银行
            int? bankId = null;
            if (channel.RequiredBank)
            {
                var bank = await bankRepository.FirstOrDefaultAsync(b => b.BankCode == input.BankCode);
                bankId = bank.Id;
            }
            //TODO 支付订单策略

            var policies = await paymentOrderPolicyService.GetPoliciesAsync(input.TenantId, channelId);

            if (policies == null)
            {
                return new ResultDto<PaymentOrderDto> { IsSuccess = false, ErrorMessage = "没有匹配的支付接口！" };
            }

            var verifiedPaymentOrder = new VerifiedPaymentOrderDto()
            {
                PaymentOrder = input
            };

            //订单提交策略验证
            foreach (var item in policies)
            {
                if (await paymentOrderPolicyService.VerifyPolicyAsync(item, input))
                {
                    verifiedPaymentOrder.Success = true;
                    verifiedPaymentOrder.CompanyId = item.CompanyId;
                    verifiedPaymentOrder.AccountId = item.AccountId;
                    verifiedPaymentOrder.ChannelId = channelId;
                    verifiedPaymentOrder.BankId = bankId;
                    break;
                }
            }

            var company = await companyRepository.GetAsync(verifiedPaymentOrder.CompanyId);
            var account = await accountRepository.GetAsync(verifiedPaymentOrder.AccountId);
            var channelOverride = await channelOverrideRepository.FirstOrDefaultAsync(co => co.CompanyId == company.Id && co.ChannelId == channelId);
            var paymentOrder = BuildPaymentOrder(verifiedPaymentOrder, GetFeeRate(company, channelOverride, account));
            await paymentOrderRepository.InsertAsync(paymentOrder);
            await CurrentUnitOfWork.SaveChangesAsync();

            return new ResultDto<PaymentOrderDto>
            {
                IsSuccess = true,
                Data = paymentOrder.MapTo<PaymentOrderDto>(),
                ErrorMessage = "提交成功"
            };
        }

        private PaymentOrder BuildPaymentOrder(VerifiedPaymentOrderDto verifiedPaymentOrder, decimal feeRate)
        {
            var input = verifiedPaymentOrder.PaymentOrder;
            var paymentOrder = new PaymentOrder()
            {
                TenantId = input.TenantId,
                Amount = input.Amount,
                Fee = Convert.ToInt32(input.Amount * (feeRate / 100)),
                CompanyId = verifiedPaymentOrder.CompanyId,
                AccountId = verifiedPaymentOrder.AccountId,
                Id = idGenerator.NextId(),
                ExternalOrderId = input.ExternalOrderId,
                ChannelId = verifiedPaymentOrder.ChannelId,
                BankId = verifiedPaymentOrder.BankId,
                CreateIp = input.CreateIp,
                AsyncCallback = input.AsyncCallback,
                SyncCallback = input.SyncCallback,
                CallbackStatus = CallbackStatus.Pending,
                Expire = Clock.Now.AddHours(2),
                ExtData = input.ExtData,
                Device = input.Device
            };
            //TODO Order MD5
            paymentOrder.Md5 = paymentOrder.GetMd5();
            paymentOrder.SetNewOrder();
            return paymentOrder;
        }

        private decimal GetFeeRate(Company company, ChannelOverride channelOverride, ThirdPartyAccount account)
        {
            if (account.OverrideFeeRate.HasValue)
                return account.OverrideFeeRate.GetValueOrDefault();
            else if (channelOverride.OverrideFeeRate.HasValue)
                return channelOverride.OverrideFeeRate.GetValueOrDefault();
            else
                return company.DefaultFeeRate.GetValueOrDefault();
        }

        private bool CheckDuplicateOrders(int tenant, string orderId)
        {
            return false;
        }
        /// <summary>
        /// 动态参数正则表达式
        /// </summary>
        private const string ParameterPattern = @"\{\{(?<key>[a-zA-Z0-9@$#&_]{1,20})\}\}";
        /// <summary>
        /// 高级动态参数正则表达式（包含JPath，XPath）
        /// </summary>
        private const string AdvancedParameterPattern = @"\{\{\{(?<key>[a-zA-Z0-9@$#&_]{1,20})\>(?<path>.*?)\}\}\}";

        public async Task<BuildOrderPostRequestResult> BuildOrderPostRequest(PaymentOrderDto paymentOrder)
        {
            var parameters = new Dictionary<string, string>();
            var apiconfig = await apiconfigRepository.FirstOrDefaultAsync(a => a.CompanyId == paymentOrder.CompanyId && a.ApiMethod == ApiMethod.PlaceOrder);
            var result = new BuildOrderPostRequestResult
            {

            };
            var entities = await apiParameterRepository.GetAll().Where(p => p.ApiId == apiconfig.Id).OrderBy(p => p.OrderNumber).ToListAsync();
            var variable = new PaymentVariable(null, null, null, null, null);
            foreach (var paymentParameter in entities)
            {
                string value = paymentParameter.Value;
                var matches = Regex.Matches(paymentParameter.Value, ParameterPattern);
                if (matches.Count > 0)
                {
                    foreach (Match item in matches)
                    {
                        var tempValue = GetVariableValue(variable, parameters, item.Groups["key"].Value, paymentParameter.Format);
                        value = value.Replace(item.Value, tempValue);
                    }
                }

                //判断必填项
                if (paymentParameter.Required && string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException($"参数{paymentParameter.Key}的值为空。");
                }
                //如果是加密参数
                if (paymentParameter.Encryption.HasValue)
                {
                    //TODO 三个密钥
                    value = EncryptionHelper.GetEncryption(paymentParameter.Encryption.Value, value, null);
                }
                if (paymentParameter.Format.Equals("ToLower", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToLower();
                }
                else if (paymentParameter.Format.Equals("ToUpper", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.ToUpper();
                }
                parameters.Add(paymentParameter.Key, value);
            }
            throw new NotImplementedException();
        }
        private string GetVariableValue(PaymentVariable variable,
           Dictionary<string, string> nameValue,
           string variableName, string format)
        {
            //检查全局变量
            if (PaymentVariable.IsGlobalVariable(variableName))
            {
                return variable.GetGlobalVariableValue(variableName, format);
            }
            else
            //检查支付配置变量
            if (PaymentVariable.IsMerchantConfigVariable(variableName))
            {
                return variable.GetMerchantConfigVariableValue(variableName, format);
            }
            else
            //检查订单变量
            if (PaymentVariable.IsOrderVariable(variableName))
            {
                return variable.GetOrderVariableValue(variableName, format);
            }
            else if (PaymentVariable.IsParameter(variableName))
            {
                return variable.GetParameterValue(nameValue, variableName, format);
            }
            else
            {
                //常量
                return variableName;
            }
        }

    }
}
