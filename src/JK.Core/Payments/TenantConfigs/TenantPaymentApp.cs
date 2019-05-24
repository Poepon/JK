using Abp.Domain.Entities;
using JK.MultiTenancy;
using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    [Table("TenantPaymentApps")]
    public class TenantPaymentApp : Entity
    {
        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public virtual Tenant Tenant { get; set; }

        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string AppId { get; set; }

        [Required]
        [StringLength(32)]
        public string Key { get; set; }

        public DeviceType Device { get; set; }

        /// <summary>
        /// 回调域名
        /// </summary>
        [StringLength(256)]
        [Required]
        public string CallbackDomain { get; set; }

        public bool UseSSL { get; set; }


        public virtual ICollection<TenantPaymentAppChannel> SupportedChannels { get; set; }

        public virtual ICollection<TenantPaymentAppCompany> SupportedCompanies { get; set; }
    }
}
