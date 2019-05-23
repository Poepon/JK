using Abp.Domain.Entities;
using JK.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    /// <summary>
    /// 租户限制策略
    /// </summary>
    public class TenantLimitPolicy : Entity
    {
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        [Required]
        [StringLength(32)]
        public string PolicyName { get; set; }


        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }

    public class TenantLimitPolicyRule : Entity
    {
        public int TenantId { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual TenantLimitPolicy Policy { get; set; }
    }

    public class TenantLimitPolicyRuleValue : Entity
    {
        public int TenantId { get; set; }

        public int RuleId { get; set; }

        [StringLength(1000)]
        public string Value { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual TenantLimitPolicyRule Rule { get; set; }
    }
}
