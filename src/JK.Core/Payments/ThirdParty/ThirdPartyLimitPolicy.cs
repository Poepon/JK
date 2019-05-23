using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JK.Payments.ThirdParty
{
    public class ThirdPartyLimitPolicy : Entity
    {
        public int CompanyId { get; set; }

        [Required]
        [StringLength(32)]
        public string PolicyName { get; set; }


        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }

    public class ThirdPartyLimitPolicyRule : Entity
    {
        public int CompanyId { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual ThirdPartyLimitPolicy Policy { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string RuleSystemName { get; set; }

        public RuleGroupInteractionType? InteractionType { get; set; }

        public bool IsGroup { get; set; }


        [ForeignKey(nameof(ParentId))]
        public virtual ICollection<ThirdPartyLimitPolicyRule> ChildPolicyRules { get; set; }
    }

    public class ThirdPartyLimitPolicyRuleValue : Entity
    {
        public int CompanyId { get; set; }

        public int RuleId { get; set; }

        public string Value { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual ThirdPartyLimitPolicyRule Rule { get; set; }
    }
}
