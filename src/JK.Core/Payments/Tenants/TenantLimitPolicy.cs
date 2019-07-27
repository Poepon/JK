using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.MultiTenancy;

namespace JK.Payments.Tenants
{
    /// <summary>
    /// 租户限制策略
    /// </summary>
    [Table("TenantLimitPolicies")]
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
    
}
