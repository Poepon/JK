using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Payments.Integration
{
    [Table("CompanyLimitPolicies")]
    public class CompanyLimitPolicy : Entity
    {
        public int CompanyId { get; set; }

        [Required]
        [StringLength(32)]
        public string PolicyName { get; set; }


        public int Priority { get; set; }

        public bool IsActive { get; set; }
    }
}
