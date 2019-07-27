using System.ComponentModel.DataAnnotations;

namespace JK.Web.Public.Dto
{
    public class PlaceOrderDto : PaymentApiDtoBase
    {

        public int Amount { get; set; }

        [Required]
        public string ChannelCode { get; set; }

        public string BankCode { get; set; }

        public string ExternalOrderId { get; set; }

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

        public override string GetSignContent()
        {
            return $"{AppId}{ChannelCode}{BankCode}{ExternalOrderId}{Amount}{SyncCallback}{AsyncCallback}{ExtData}{NonceStr}";
        }
    }
}
