using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JK.Payments.ThirdParty.Dto
{
    [AutoMap(typeof(Company))]
    public class CompanyDto : EntityDto
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
    }
}
