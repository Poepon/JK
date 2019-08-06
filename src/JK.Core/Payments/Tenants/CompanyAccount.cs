using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using JK.MultiTenancy;
using JK.Payments.Integration;

namespace JK.Payments.Tenants
{
    /// <summary>
    /// 第三方帐户配置
    /// </summary>
    [Table("CompanyAccounts")]
    public class CompanyAccount : FullAuditedEntity
    {
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        [Required]
        [StringLength(32)]
        public string MerchantId { get; set; }

        [Required]
        [StringLength(32)]
        public string MerchantKey { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal? OverrideFeeRate { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public long Balance { get; set; }

    }
}
