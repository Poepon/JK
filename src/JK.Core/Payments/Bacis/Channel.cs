using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Integration;
using JK.Payments.Orders;
using JK.Payments.Tenants;

namespace JK.Payments.Bacis
{
    /// <summary>
    /// 支付通道
    /// </summary>
    [Table("Channels")]
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

        public virtual ICollection<ApiChannel> ApiChannels { get; set; }

        public virtual ICollection<CompanyChannel> CompanyChannels { get; set; }

        public virtual ICollection<PaymentOrderPolicyChannel> PaymentOrderPolicyChannels { get; set; }

        public virtual ICollection<PaymentAppChannel> AppChannels { get; set; }

        public virtual ICollection<PaymentAppCompany> AppCompanies { get; set; }

        public virtual ICollection<ParameterChannel> ParameterChannels { get; set; }

    }
}
