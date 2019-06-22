using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Bacis;

namespace JK.Payments
{
    [AutoMap(typeof(Channel))]
    public class ChannelDto : EntityDto
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(20)]
        public string ChannelCode { get; set; }

        /// <summary>
        /// 是否要求银行
        /// </summary>
        public bool RequiredBank { get; set; }

        public bool IsActive { get; set; }
    }
}
