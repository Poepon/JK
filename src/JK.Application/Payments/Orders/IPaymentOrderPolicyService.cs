using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Repositories;
using JK.Payments.Orders.Dto;

namespace JK.Payments.Orders
{
    public interface IPaymentOrderPolicyService
    {
        List<PaymentOrderPolicy> GetPolicies(int tenantId, int channelId);

        bool VerifyPolicy(PaymentOrderPolicy paymentOrderPolicy, CreatePaymentOrderDto input);
    }
    public class PaymentOrderPolicyService : IPaymentOrderPolicyService
    {
        private readonly IRepository<PaymentOrderPolicy> policyRepository;

        public PaymentOrderPolicyService(IRepository<PaymentOrderPolicy> policyRepository)
        {
            this.policyRepository = policyRepository;
        }
        public List<PaymentOrderPolicy> GetPolicies(int tenantId, int channelId)
        {
            var list = policyRepository.GetAll().Where(p => p.TenantId == tenantId && p.IsActive &&
             p.Company.IsActive &&
             p.SupportedChannels.Any(sup => sup.ChannelId == channelId))
               .OrderBy(p => p.Priority).ToList();
            return list;
        }

        public bool VerifyPolicy(PaymentOrderPolicy paymentOrderPolicy, CreatePaymentOrderDto input)
        {
            if (paymentOrderPolicy == null)
                throw new ArgumentNullException(nameof(paymentOrderPolicy));
            throw new NotImplementedException();
        }
    }
}
