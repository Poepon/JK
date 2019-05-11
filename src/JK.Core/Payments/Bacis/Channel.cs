using System;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.Bacis
{
    /// <summary>
    /// 支付通道
    /// </summary>
    public class Channel
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(20)]
        public string ChannelCode { get; set; }

        public bool IsActive { get; set; }
    }
}
