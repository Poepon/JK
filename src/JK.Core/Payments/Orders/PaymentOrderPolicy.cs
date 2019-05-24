using Abp.Domain.Entities;
using JK.MultiTenancy;
using JK.Payments.Integration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.Orders
{
    /// <summary>
    /// 支付订单下单策略
    /// </summary>
    [Table("PaymentOrderPolicies")]
    public class PaymentOrderPolicy : Entity
    {
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        [Required]
        [StringLength(32)]
        public string PolicyName { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public int AccountId { get; set; }
        

        public int Priority { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<PaymentOrderPolicyChannel> SupportedChannels { get; set; }
    }
}
