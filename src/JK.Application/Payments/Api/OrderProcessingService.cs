using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Timing;
using JK.Dto;
using JK.Payments.Bacis;
using JK.Payments.Enumerates;
using JK.Payments.Integration;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using Microsoft.EntityFrameworkCore;

namespace JK.Payments.Orders
{
    public class OrderProcessingService : AbpServiceBase, IOrderProcessingService
    {
        private readonly ITenantCache tenantCache;
        private readonly IPaymentOrderPolicyService paymentOrderPolicyService;
        private readonly IIdGenerator idGenerator;
        private readonly IRepository<Company> companyRepository;
        private readonly IRepository<Channel> channelRepository;
        private readonly IRepository<Bank> bankRepository;
        private readonly IRepository<ChannelOverride> channelOverrideRepository;
        private readonly IRepository<CompanyAccount> accountRepository;
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
            IRepository<CompanyAccount> accountRepository,
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
                AppId = input.AppId,
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
                PaymentMode = input.PaymentMode
            };
            paymentOrder.Md5 = paymentOrder.GetMd5();
            paymentOrder.SetNewOrder();
            return paymentOrder;
        }

        private decimal GetFeeRate(Company company, ChannelOverride channelOverride, CompanyAccount account)
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

        public async Task<BuildOrderPostRequestResult> BuildOrderPostRequest(PaymentOrderDto paymentOrder)
        {
            var apiconfig = await apiconfigRepository.FirstOrDefaultAsync(a => a.CompanyId == paymentOrder.CompanyId &&
                                                                               a.ApiMethod == ApiMethod.PlaceOrder &&
                                                                               a.SupportedChannels.Any(sc => sc.ChannelId == paymentOrder.ChannelId));
            var result = new BuildOrderPostRequestResult
            {
                ApiId = apiconfig.Id,
                Url = apiconfig.Url,
                AcceptCharset = apiconfig.AcceptCharset,
                ContentType = apiconfig.ContentType,
                DataType = apiconfig.DataType,
                Method = apiconfig.Method,
                RequestType = apiconfig.RequestType,
                HasResponeParameter = apiconfig.HasResponeParameter
            };
            var apiRequests = await apiParameterRepository.GetAll()
                .Where(p => p.CompanyId == paymentOrder.CompanyId && p.ApiMethod == ApiMethod.PlaceOrder && p.ParameterType == ParameterType.Request &&
                            p.SupportedChannels.Any(c => c.ChannelId == paymentOrder.ChannelId))
                .OrderBy(p => p.OrderNumber).ToListAsync();
            if (apiconfig.HasResponeParameter)
            {
                var apiRespones = await apiParameterRepository.GetAll()
                    .Where(p => p.CompanyId == paymentOrder.CompanyId && p.ApiMethod == ApiMethod.PlaceOrder && p.ParameterType == ParameterType.Respone &&
                                p.SupportedChannels.Any(c => c.ChannelId == paymentOrder.ChannelId))
                    .OrderBy(p => p.OrderNumber).ToListAsync();
                result.ApiResponeParameters = apiRespones;
            }
            //TODO PaymentVariable
            var variable = new PaymentVariable(null, null, null, null, null);
            //TODO 区分Query Content Headers
            var values = variable.ProcessingApiRequestParameters(apiRequests);
            if (values.ContainsKey(SetValueLocation.Query))
            {
                result.Query = values[SetValueLocation.Query];
            }
            if (values.ContainsKey(SetValueLocation.Headers))
            {
                result.Headers = values[SetValueLocation.Headers];
            }
            if (values.ContainsKey(SetValueLocation.Content))
            {
                result.Content = values[SetValueLocation.Content];
            }
            return result;
        }

        public async Task<List<ApiParameter>> GetOrderCallbackParametersAsync(int companyId,int channelId)
        {
            return await apiParameterRepository.GetAll()
                .Where(p => p.CompanyId == companyId && p.ApiMethod == ApiMethod.PlaceOrder && p.ParameterType == ParameterType.Callback &&
                            p.SupportedChannels.Any(c => c.ChannelId == channelId))
                .OrderBy(p => p.OrderNumber)
                .ToListAsync();
        }
        [UnitOfWork]
        public virtual async Task<ResultDto<PaymentStatus>> MarkOrderAsPaid(long orderId)
        {
            var paymentOrder = await paymentOrderRepository.GetAsync(orderId);
            if (paymentOrder == null)
            {
                return new ResultDto<PaymentStatus>() { IsSuccess = false, ErrorMessage = "订单不存在" };
            }
            if (!paymentOrder.VerifyMd5())
            {
                return new ResultDto<PaymentStatus>() { IsSuccess = false, ErrorMessage = "订单数据已更改" };
            }
            if (paymentOrder.PaymentStatus == PaymentStatus.Pending)
            {
                paymentOrder.ChangePaymentStatus(PaymentStatus.Paid);
                await paymentOrderRepository.UpdateAsync(paymentOrder);
                await CurrentUnitOfWork.SaveChangesAsync();
                return new ResultDto<PaymentStatus>() { IsSuccess = true, ErrorMessage = "订单已支付", Data = PaymentStatus.Paid };
            }
            else if (paymentOrder.PaymentStatus == PaymentStatus.Paid)
            {
                return new ResultDto<PaymentStatus>() { IsSuccess = true, ErrorMessage = "订单已支付", Data = PaymentStatus.Paid };
            }
            else
            {
                return new ResultDto<PaymentStatus>() { IsSuccess = false, ErrorMessage = "订单已取消", Data = PaymentStatus.Cancelled };
            }
        }
    }
}