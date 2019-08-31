using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Application.Services.Dto;
using AutoMapper;
using JK.MultiTenancy;
using JK.Payments.Enumerates;
using JK.Payments.Tenants;

namespace JK.Payments.TenantConfigs.Dto
{
    [AutoMap(typeof(PaymentApp))]
    public class PaymentAppDto : EntityDto
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string AppId { get; set; }

        /// <summary>
        /// 验签密钥
        /// </summary>
        [Required]
        [StringLength(32)]
        public string Key { get; set; }

        /// <summary>
        /// 透明密钥（内部数据防篡改）
        /// </summary>
        [Required]
        [StringLength(32)]
        public string TransparentKey { get; set; }

        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 回调域名
        /// </summary>
        [StringLength(256)]
        [Required]
        public string CallbackDomain { get; set; }

        public bool UseSSL { get; set; }

        public bool IsActive { get; set; }

    }
}
