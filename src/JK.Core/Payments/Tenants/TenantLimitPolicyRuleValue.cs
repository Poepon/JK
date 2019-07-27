using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Payments.Tenants
{

    [Table("TenantLimitPolicyRuleValues")]
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
