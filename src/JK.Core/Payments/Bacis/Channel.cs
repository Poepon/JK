using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.Bacis
{
    /// <summary>
    /// 支付通道
    /// </summary>
    public class Channel : Entity
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
