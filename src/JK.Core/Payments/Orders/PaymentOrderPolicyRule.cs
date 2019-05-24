using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Orders
{
    /// <summary>
    /// 支付订单下单策略规则
    /// </summary>
    [Table("PaymentOrderPolicyRules")]
    public class PaymentOrderPolicyRule : Entity
    {
        public int TenantId { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual PaymentOrderPolicy Policy { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string RuleSystemName { get; set; }

        public RuleGroupInteractionType? InteractionType { get; set; }

        public bool IsGroup { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual ICollection<PaymentOrderPolicyRule> ChildPolicyRules { get; set; }
    }
}