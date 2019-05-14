using System;
using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Timing;
using JK.Dto;
using JK.Payments.Bacis;
using JK.Payments.Enumerates;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Payments.ThirdParty;

namespace JK.Payments.Orders
{
    public interface IOrderProcessingService
    {

        /// <summary>
        /// 提交一个支付请求
        /// </summary>
        ResultDto<PaymentOrderDto> PlaceOrder(CreatePaymentOrderDto input);
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

        public OrderProcessingService(
            ITenantCache tenantCache,
            IPaymentOrderPolicyService paymentOrderPolicyService,
            IIdGenerator idGenerator,
            IRepository<Company> companyRepository,
            IRepository<Channel> channelRepository,
            IRepository<ChannelOverride> channelOverrideRepository,
            IRepository<Bank> bankRepository,
            IRepository<ThirdPartyAccount> accountRepository,
            IRepository<PaymentOrder, long> paymentOrderRepository)
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
        }

        public ResultDto<PaymentOrderDto> PlaceOrder(CreatePaymentOrderDto input)
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
            var channel = channelRepository.FirstOrDefault(c => c.ChannelCode == input.ChannelCode);
            int channelId = channel.Id;
            //银行
            int? bankId = null;
            if (channel.RequiredBank)
            {
                var bank = bankRepository.FirstOrDefault(b => b.BankCode == input.BankCode);
                bankId = bank.Id;
            }
            //TODO 支付订单策略

            var policies = paymentOrderPolicyService.GetPolicies(input.TenantId, channelId);

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
                if (paymentOrderPolicyService.VerifyPolicy(item, input))
                {
                    verifiedPaymentOrder.Success = true;
                    verifiedPaymentOrder.CompanyId = item.CompanyId;
                    verifiedPaymentOrder.AccountId = item.AccountId;
                    verifiedPaymentOrder.ChannelId = channelId;
                    verifiedPaymentOrder.BankId = bankId;
                    break;
                }
            }

            var company = companyRepository.Get(verifiedPaymentOrder.CompanyId);
            var account = accountRepository.Get(verifiedPaymentOrder.AccountId);
            var channelOverride = channelOverrideRepository.FirstOrDefault(co => co.CompanyId == company.Id && co.ChannelId == channelId);
            var paymentOrder = BuildPaymentOrder(verifiedPaymentOrder, GetFeeRate(company, channelOverride, account));
            paymentOrderRepository.Insert(paymentOrder);
            CurrentUnitOfWork.SaveChanges();

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
    }
}
