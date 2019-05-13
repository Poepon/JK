using System;
using System.Collections.Generic;
using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using Abp.Timing;
using JK.Dto;
using JK.Payments.Enumerates;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Payments.ThirdParty;

namespace JK.Payments.Orders
{
    public interface IPaymentOrderPolicyService
    {
        List<PaymentOrderPolicy> GetPolicies(int tenantId, int channelId);

        bool VerifyPolicy(PaymentOrderPolicy paymentOrderPolicy, CreatePaymentOrderDto input);
    }
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
        private readonly IRepository<ThirdPartyAccount> accountRepository;
        private readonly IRepository<PaymentOrder, long> paymentOrderRepository;

        public OrderProcessingService(
            ITenantCache tenantCache,
            IPaymentOrderPolicyService paymentOrderPolicyService,
            IIdGenerator idGenerator,
            IRepository<Company> companyRepository,
            IRepository<ThirdPartyAccount> accountRepository,
            IRepository<PaymentOrder, long> paymentOrderRepository)
        {
            this.tenantCache = tenantCache;
            this.paymentOrderPolicyService = paymentOrderPolicyService;
            this.idGenerator = idGenerator;
            this.companyRepository = companyRepository;
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
            //TODO 支付通道
            int channelId = input.ChannelCode.GetHashCode();

            //TODO 银行
            int? bankId = input.BankCode.GetHashCode();

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
            var paymentOrder = BuildPaymentOrder(verifiedPaymentOrder, GetFeeRate(company, account));
            paymentOrderRepository.Insert(paymentOrder);
            CurrentUnitOfWork.SaveChanges();

            return new ResultDto<PaymentOrderDto>
            {
                IsSuccess = true,
                Data = paymentOrder.MapTo<PaymentOrderDto>(),
                ErrorMessage = "提交成功"
            };
            throw new NotImplementedException();
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
                SystemOrderId = idGenerator.NextId(),
                ExternalOrderId = input.ExternalOrderId,
                ChannelId = verifiedPaymentOrder.ChannelId,
                BankId = verifiedPaymentOrder.BankId,
                CreateIp = input.CreateIp,
                AsyncCallback = input.AsyncCallback,
                SyncCallback = input.SyncCallback,
                CallbackStatus = CallbackStatus.Pending,
                Expire = Clock.Now.AddHours(2),
                ExtData = input.ExtData,
                Md5 = "",
                Device = ""
            };
            //TODO Order MD5

            //Insert into Database
            paymentOrder.ChangePaymentStatus(PaymentStatus.Pending);
            return paymentOrder;
        }

        private decimal GetFeeRate(Company company, ThirdPartyAccount account)
        {
            if (account.FeeRate.HasValue)
                return account.FeeRate.GetValueOrDefault();
            else
                return company.FeeRate.GetValueOrDefault();
        }

        private bool CheckDuplicateOrders(int tenant, string orderId)
        {
            return false;
        }
    }
}
