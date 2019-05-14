using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// 支付公司
    /// </summary>
    public class Company : Entity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public CurrencyUnit CurrencyUnit { get; set; }

        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal? DefaultFeeRate { get; set; }

        public long? MinOrderAmount { get; set; }

        public long? MaxOrderAmount { get; set; }

        public virtual ICollection<CompanyChannel> SupportedChannels { get; set; }
    }
}
