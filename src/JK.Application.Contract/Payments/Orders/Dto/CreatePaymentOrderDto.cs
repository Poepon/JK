using JK.Payments.Enumerates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JK.Payments.Orders.Dto
{
    public class CreatePaymentOrderDto
    {
        public int TenantId { get; set; }

        public int AppId { get; set; }

        public string TransparentKey { get; set; }

        public int Amount { get; set; }

        [Required]
        public string ChannelCode { get; set; }

        public string BankCode { get; set; }

        public string ExternalOrderId { get; set; }

        [Required]
        [StringLength(64)]
        public string CreateIp { get; set; }

        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 同步回调
        /// </summary>
        [StringLength(2000)]
        public string SyncCallback { get; set; }

        /// <summary>
        /// 异步回调
        /// </summary>
        [StringLength(2000)]
        public string AsyncCallback { get; set; }

        /// <summary>
        /// 扩展数据，原样返回
        /// </summary>
        public string ExtData { get; set; }

        public void Normalize()
        {
            if (CreateIp == "::1")
            {
                CreateIp = "127.0.0.1";
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Amount <= 0)
            {
                yield return new ValidationResult("订单金额不能小于等于0！", new[] { "Amount" });
            }
            //if (!ValidationHelper.IsUrl(AsyncCallback))
            //{
            //    yield return new ValidationResult("异步回调地址不正确！", new[] { "AsyncCallback" });
            //}
        }
    }
}
