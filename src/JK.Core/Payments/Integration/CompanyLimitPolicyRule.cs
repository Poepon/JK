using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    [Table("CompanyLimitPolicyRules")]
    public class CompanyLimitPolicyRule : Entity
    {
        public int CompanyId { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual CompanyLimitPolicy Policy { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string RuleSystemName { get; set; }

        public RuleGroupInteractionType? InteractionType { get; set; }

        public bool IsGroup { get; set; }


        [ForeignKey(nameof(ParentId))]
        public virtual ICollection<CompanyLimitPolicyRule> ChildPolicyRules { get; set; }
    }
}
