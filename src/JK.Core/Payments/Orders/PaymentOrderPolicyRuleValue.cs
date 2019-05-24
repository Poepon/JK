using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace JK.Payments.Orders
{
    [Table("PaymentOrderPolicyRuleValues")]
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