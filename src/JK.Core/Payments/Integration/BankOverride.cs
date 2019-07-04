using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Bacis;

namespace JK.Payments.Integration
{
    [Table("BankOverrides")]
    public class BankOverride : Entity
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public int BankId { get; set; }

        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }

        [Required]
        [StringLength(16)]
        public string OverrideCode { get; set; }

        public bool? OverrideIsActive { get; set; }
    }
}
