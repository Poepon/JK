using Abp.Domain.Entities.Auditing;
using JK.Payments.ThirdParty;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    /// <summary>
    /// 第三方帐户配置
    /// </summary>
    public class ThirdPartyAccount : FullAuditedEntity
    {
        public int TenantId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [StringLength(50)]
        public string MerchantId { get; set; }

        /// <summary>
        /// 商户帐号
        /// </summary>
        [StringLength(50)]
        public string MerchantAccount { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        [StringLength(200)]
        public string MerchantKey { get; set; }

        /// <summary>
        /// 数据加密公钥
        /// </summary>
        [StringLength(1000)]
        public string PublicKey { get; set; }

        /// <summary>
        /// 数据加密私钥
        /// </summary>
        [StringLength(1000)]
        public string PrivateKey { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal? OverrideFeeRate { get; set; }

    }
}
