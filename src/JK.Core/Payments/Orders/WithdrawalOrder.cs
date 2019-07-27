using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using JK.Payments.Enumerates;

namespace JK.Payments.Orders
{
    public class WithdrawalOrder : AuditedAggregateRoot<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        public int TenantId { get; set; }

        public int CompanyId { get; set; }

        public int AccountId { get; set; }

        public int AppId { get; set; }

        public int? BankId { get; set; }

        /// <summary>
        /// 外部订单号
        /// </summary>
        [StringLength(32)]
        public string ExternalOrderId { get; set; }

        /// <summary>
        /// 第三方订单号
        /// </summary>
        [StringLength(32)]
        public string ThirdPartyOrderId { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// 回调状态
        /// </summary>
        public CallbackStatus CallbackStatus { get; set; }


        [StringLength(500)]
        public string ExtData { get; set; }

        /// <summary>
        /// 同步回调
        /// </summary>
        [StringLength(256)]
        public string SyncCallback { get; set; }

        /// <summary>
        /// 异步回调
        /// </summary>
        [StringLength(256)]
        public string AsyncCallback { get; set; }
    }
}