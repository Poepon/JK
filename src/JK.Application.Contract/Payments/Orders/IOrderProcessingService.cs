using JK.Dto;
using JK.Payments.Enumerates;
using JK.Payments.Orders.Dto;
using System.Collections.Generic;
using System.Linq;
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

        public void SetHttpValues(ParameterValueResult values)
        {
            var groups = values.ToLookup(v => v.Location);
            if (groups.Contains(Location.SetContent))
            {
                Content = new Dictionary<string, string>();
                foreach (var value in groups[Location.SetContent])
                {
                    Content.Add(value.Key, value.Value);
                }
            }
            if (groups.Contains(Location.SetHeaders))
            {
                Headers = new Dictionary<string, string>();
                foreach (var value in groups[Location.SetHeaders])
                {
                    Headers.Add(value.Key, value.Value);
                }
            }
            if (groups.Contains(Location.SetQuery))
            {
                Query = new Dictionary<string, string>();
                foreach (var value in groups[Location.SetQuery])
                {
                    Query.Add(value.Key, value.Value);
                }
            }
        }

        public List<ApiParameter> ApiResponeParameters { get; set; }
    }
    public interface IOrderProcessingService
    {

        /// <summary>
        /// 提交一个支付请求
        /// </summary>
        Task<ResultDto<PaymentOrderDto>> PlaceOrderAsync(CreatePaymentOrderDto input);

        Task<BuildOrderPostRequestResult> BuildOrderPostRequest(PaymentOrderDto paymentOrder);

        Task<List<ApiParameter>> GetApiParameters(int companyId, int channelId, ApiMethod method, ParameterType parameterType);

        Task<ResultDto<PaymentStatus>> MarkOrderAsPaid(long orderId);
    }
}
