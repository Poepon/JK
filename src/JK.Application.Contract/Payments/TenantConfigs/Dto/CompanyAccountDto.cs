using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JK.Payments.TenantConfigs.Dto
{
    public class CompanyAccountDto : FullAuditedEntityDto
    {
        public CompanyAccountDto()
        {
            Attributes = new Dictionary<string, string>();
        }
        public int TenantId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int CompanyId { get; set; }


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

        public virtual Dictionary<string, string> Attributes { get; set; }

        public string this[string key]
        {
            get
            {
                if (key == nameof(MerchantId))
                    return MerchantId;
                else if (key == nameof(MerchantKey))
                    return MerchantKey;
                else
                    return Attributes[key];
            }
            set
            {
                if (key == nameof(MerchantId))
                    MerchantId = value;
                else if (key == nameof(MerchantKey))
                    MerchantKey = value;
                else
                    Attributes[key] = value;
            }
        }
    }
}
