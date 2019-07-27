using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Payments.Tenants
{
    [Table("TenantLimitPolicyRules")]
    public class TenantLimitPolicyRule : Entity
    {
        public int TenantId { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual TenantLimitPolicy Policy { get; set; }
    }
    
}
