using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using JK.Payments.Orders.Dto;
using Microsoft.EntityFrameworkCore;

namespace JK.Payments.Orders
{
    public interface IPaymentOrderPolicyService
    {
        Task<List<PaymentOrderPolicy>> GetPoliciesAsync(int tenantId, int channelId);

        Task<bool> VerifyPolicyAsync(PaymentOrderPolicy paymentOrderPolicy, CreatePaymentOrderDto input);
    }
    public class PaymentOrderPolicyService : IPaymentOrderPolicyService
    {
        private readonly IRepository<PaymentOrderPolicy> policyRepository;

        public PaymentOrderPolicyService(IRepository<PaymentOrderPolicy> policyRepository)
        {
            this.policyRepository = policyRepository;
        }
        public async Task<List<PaymentOrderPolicy>> GetPoliciesAsync(int tenantId, int channelId)
        {
            var list = await policyRepository.GetAll().Where(p => p.TenantId == tenantId && p.IsActive &&
              p.Company.IsActive &&
              p.SupportedChannels.Any(sup => sup.ChannelId == channelId))
               .OrderBy(p => p.Priority).ToListAsync();
            return list;
        }

        public Task<bool> VerifyPolicyAsync(PaymentOrderPolicy paymentOrderPolicy, CreatePaymentOrderDto input)
        {
            if (paymentOrderPolicy == null)
                throw new ArgumentNullException(nameof(paymentOrderPolicy));

            throw new NotImplementedException();
        }
    }
}
