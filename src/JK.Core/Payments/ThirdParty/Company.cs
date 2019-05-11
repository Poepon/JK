using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// 支付公司
    /// </summary>
    public class Company
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public CurrencyUnit CurrencyUnit { get; set; }

        public virtual ICollection<CompanyChannel> SupportedChannels { get; set; }
    }
}
