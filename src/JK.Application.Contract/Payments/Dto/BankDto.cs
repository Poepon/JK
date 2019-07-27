using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Bacis;

namespace JK.Payments.Dto
{
    public class BankDto : EntityDto
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string BankCode { get; set; }

        public int OrderNumber { get; set; }
    }
}
