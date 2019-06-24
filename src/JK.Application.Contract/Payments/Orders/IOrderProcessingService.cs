using JK.Dto;
using JK.Payments.Enumerates;
using JK.Payments.Orders.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using JK.Payments.Integration;

namespace JK.Payments.Orders
{
    public class BuildOrderPostRequestResult
    {
        public int ApiId { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public RequestType RequestType { get; set; }

        public string ContentType { get; set; }

        public DataType DataType { get; set; }

        public string AcceptCharset { get; set; }

        public bool HasResponeParameter { get; set; }

        public Dictionary<string, string> Content { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Dictionary<string, string> Query { get; set; }

        public List<ApiParameter> ApiResponeParameters { get; set; }
    }
    public interface IOrderProcessingService
    {

        /// <summary>
        /// 提交一个支付请求
        /// </summary>
        Task<ResultDto<PaymentOrderDto>> PlaceOrderAsync(CreatePaymentOrderDto input);

        Task<BuildOrderPostRequestResult> BuildOrderPostRequest(PaymentOrderDto paymentOrder);

        Task<List<ApiParameter>> GetOrderCallbackParametersAsync(int companyId,int channelId);

        Task<ResultDto<PaymentStatus>> MarkOrderAsPaid(long orderId);
    }
}
