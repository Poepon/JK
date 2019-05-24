using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Payments.Integration
{
    [Table("CompanyLimitPolicyRuleValues")]
    public class CompanyLimitPolicyRuleValue : Entity
    {
        public int CompanyId { get; set; }

        public int RuleId { get; set; }

        public string Value { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual CompanyLimitPolicyRule Rule { get; set; }
    }
}
