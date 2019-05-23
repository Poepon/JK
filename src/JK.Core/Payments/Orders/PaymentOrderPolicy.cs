using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using JK.MultiTenancy;
using JK.Payments.Bacis;
using JK.Payments.Enumerates;
using JK.Payments.TenantConfigs;
using JK.Payments.ThirdParty;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.Orders
{
    /// <summary>
    /// 支付订单下单策略
    /// </summary>
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

        [ForeignKey(nameof(AccountId))]
        public virtual ThirdPartyAccount Account { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<PaymentOrderPolicyChannel> SupportedChannels { get; set; }
    }

    public class PaymentOrderPolicyChannel
    {
        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual PaymentOrderPolicy PaymentOrderPolicy { get; set; }
    }

    /// <summary>
    /// 支付订单下单策略规则
    /// </summary>
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

    public class PaymentOrderPolicyRuleValue : AuditedEntity
    {
        /// <summary>
        /// Maximum length of the <see cref="Value"/> property.
        /// </summary>
        public const int MaxValueLength = 1000;

        public int TenantId { get; set; }

        public int RuleId { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual PaymentOrderPolicyRule Rule { get; set; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        [StringLength(MaxValueLength)]
        public string Value { get; set; }
    }

}
