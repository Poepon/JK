using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Bacis;

namespace JK.Payments.Dto
{
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
        /// 是否需要指定银行
        /// </summary>
        public bool RequiredBank { get; set; }

        public bool IsActive { get; set; }
    }
}
