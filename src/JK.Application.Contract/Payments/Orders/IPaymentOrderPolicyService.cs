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

  
}
