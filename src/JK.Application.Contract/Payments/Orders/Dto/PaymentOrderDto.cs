using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Enumerates;

namespace JK.Payments.Orders.Dto
{
    [AutoMap(typeof(PaymentOrder))]
    public class PaymentOrderDto : FullAuditedEntityDto<long>
    {
        public int TenantId { get; set; }

        public int CompanyId { get; set; }

        public int ChannelId { get; set; }

        public int AccountId { get; set; }

        public int? BankId { get; set; }

        /// <summary>
        /// 外部订单号
        /// </summary>
        public string ExternalOrderId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        public string ThirdPartyOrderId { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public long? PaidAmount { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public long Fee { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expire { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaidDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        /// <summary>
        /// 回调状态
        /// </summary>
        public CallbackStatus CallbackStatus { get; set; }

        public string CreateIp { get; set; }

        public PaymentMode PaymentMode { get; set; }

        public string Md5 { get; set; }

        /// <summary>
        /// 同步回调
        /// </summary>
        public string SyncCallback { get; set; }

        /// <summary>
        /// 异步回调
        /// </summary>
        public string AsyncCallback { get; set; }

        public string ExtData { get; set; }
    }
}
