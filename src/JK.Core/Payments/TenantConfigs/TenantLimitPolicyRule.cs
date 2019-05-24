using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
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
