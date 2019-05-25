using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using JK.Payments.Enumerates;
using JK.Payments.Evens;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.Orders
{
    [Table("PaymentOrders")]
    public class PaymentOrder : AuditedAggregateRoot<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        public int TenantId { get; set; }
        
        public int CompanyId { get; set; }
        
        public int ChannelId { get; set; }
        
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

        [Required]
        [StringLength(64)]
        public string CreateIp { get; set; }

        public PaymentMode PaymentMode { get; set; }

        [Required]
        [StringLength(32)]
        public string Md5 { get; set; }

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

        [StringLength(500)]
        public string ExtData { get; set; }

        public void SetNewOrder()
        {
            this.PaymentStatus = PaymentStatus.Pending;
            this.CreationTime = Clock.Now;
            this.DomainEvents.Add(new PaymentOrderCreatedEventData(this));
        }

        public void ChangePaymentStatus(PaymentStatus paymentStatus)
        {
            this.PaymentStatus = paymentStatus;
            switch (paymentStatus)
            {
                case PaymentStatus.Paid:
                    PaidDate = Clock.Now;
                    this.DomainEvents.Add(new PaymentOrderPaidEventData(this));
                    break;
                case PaymentStatus.Cancelled:
                    this.CancelledDate = Clock.Now;
                    this.DomainEvents.Add(new PaymentOrderCancelledEventData(this));
                    break;
                default:
                    break;
            }
        }
        public void ChangeCallbackStatus(CallbackStatus callbackStatus)
        {
            var oldStatus = CallbackStatus;
            this.CallbackStatus = callbackStatus;
            this.DomainEvents.Add(new PaymentOrderCallbackStatusChangedEventData(this, oldStatus));
        }
        public void ChangeCallbackStatus(CallbackStatus callbackStatus, string reason)
        {
            var oldStatus = CallbackStatus;
            this.CallbackStatus = callbackStatus;
            this.DomainEvents.Add(new PaymentOrderCallbackStatusChangedEventData(this, oldStatus, reason));
        }
    }
}
