using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Runtime.Session;
using JK.Cryptography;
using JK.Payments;
using JK.Payments.Enumerates;
using JK.Payments.Integration;
using JK.Payments.Orders;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Web.Public.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace JK.Web.Public.Controllers
{
    [Route("api/pay/[action]")]
    public class PaymentApiController : AbpController
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepository<TenantPaymentApp> appRepository;
        private readonly IOrderProcessingService orderProcessingService;
        private readonly IRepository<ResultCodeConfiguration> resultCodeRepository;
        public PaymentApiController(
            IHttpClientFactory httpClientFactory,
            IRepository<TenantPaymentApp> appRepository,
            IRepository<ResultCodeConfiguration> resultCodeRepository,
            IOrderProcessingService orderProcessingService)
        {
            this.httpClientFactory = httpClientFactory;
            this.appRepository = appRepository;
            this.resultCodeRepository = resultCodeRepository;
            this.orderProcessingService = orderProcessingService;
        }

        protected Task<TenantPaymentApp> QueryApp(PaymentApiDtoBase input)
        {
            return appRepository.FirstOrDefaultAsync(a => a.AppId == input.AppId);
        }

        /// <summary>
        /// 验签
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool VerifySign(string sign, string signContent, string key)
        {
            return sign == SecurityHelper.MD5(signContent, key);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderDto input)
        {
            var app = await QueryApp(input);
            string signContent = input.GetSignContent();
            if (VerifySign(input.Sign, signContent, app.Key))
            {
                var result = await orderProcessingService.PlaceOrderAsync(BuildOrder(input, app));
                if (result.IsSuccess)
                {
                    return await PostOrder(result.Data);
                }
                return Content("验签成功");
            }
            else
            {
                return Content("验签失败");
            }
        }

        protected async Task<IActionResult> PostOrder(PaymentOrderDto paymentOrder)
        {
            var postdata = await orderProcessingService.BuildOrderPostRequest(paymentOrder);
            if (postdata.Query != null && postdata.Query.Count > 0)
            {
                postdata.Url = QueryHelpers.AddQueryString(postdata.Url, postdata.Query);
            }
            if (postdata.RequestType == RequestType.WebPage)
            {
                if (postdata.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    return View(postdata);
                }
                else
                {
                    return Redirect(postdata.Url);
                }
            }
            else
            {
                var httpClient = httpClientFactory.CreateClient();
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(
                  postdata.Method.Equals("post", StringComparison.OrdinalIgnoreCase) ?
                    HttpMethod.Post :
                    HttpMethod.Get, postdata.Url);
                if (postdata.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    if (postdata.ContentType == "application/json")
                        httpRequestMessage.Content = new StringContent(postdata.Content.ToJsonString());
                    else if (postdata.ContentType == "text/xml")
                        httpRequestMessage.Content = new StringContent(postdata.Content.ToXmlString());
                    else
                        httpRequestMessage.Content = new FormUrlEncodedContent(postdata.Content);
                }
                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    if (postdata.HasResponeParameter)
                    {
                        var data = new ConcurrentDictionary<string, string>();
                        await ProcessingData(data, await response.Content.ReadAsStringAsync(), postdata.DataType, postdata.ApiResponeParameters);
                        var resultCode = data[DataTag.ResultCode.ToString()];
                        var resultCodeMean = await resultCodeRepository.FirstOrDefaultAsync(c => c.ResultCode == resultCode && c.CompanyId == paymentOrder.CompanyId);
                        if (resultCodeMean == null)
                            throw new Exception($"ResultCode:{resultCode}未配置。");
                        if (resultCodeMean.Mean == ResultCodeMean.Succeed)
                        {
                            if (data.ContainsKey(DataTag.PayAppUrl.ToString()))
                            {
                                return Redirect(data[DataTag.PayAppUrl.ToString()]);
                            }
                            else if (data.ContainsKey(DataTag.PayQrCode.ToString()))
                            {
                                return RedirectToAction("QrCode", "Payment", new
                                {
                                    url = data[DataTag.PayQrCode.ToString()],
                                    type = StaticContext.Channels.Find(c => c.Id == paymentOrder.ChannelId).DisplayName,
                                    amount = (paymentOrder.Amount / 100m).ToString()
                                });
                            }
                            else
                            {
                                return Content("不支持的支付类型。");
                            }
                        }
                        else
                        {
                            return Content(data[DataTag.ResultDesc.ToString()]);
                        }
                    }
                }
                return Content("支付提交失败！");
            }
        }

        private CreatePaymentOrderDto BuildOrder(PlaceOrderDto input, TenantPaymentApp app)
        {
            return new CreatePaymentOrderDto
            {
                TenantId = app.TenantId,
                AppId = app.Id,
                ChannelCode = input.ChannelCode,
                Amount = input.Amount,
                BankCode = input.BankCode,
                CreateIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                PaymentMode = app.PaymentMode,
                ExternalOrderId = input.ExternalOrderId,
                SyncCallback = input.SyncCallback,
                AsyncCallback = input.AsyncCallback,
                ExtData = input.ExtData
            };
        }

        private async Task ProcessingData<T>(ConcurrentDictionary<string, string> values, string dataContent, DataType dataType, IEnumerable<T> responeParameters) where T : IValueParameter
        {

            if (dataType == DataType.Json)
            {
                ProcessingJsonData(responeParameters, values, dataContent);
            }
            else if (dataType == DataType.Xml)
            {
                ProcessingXmlData(responeParameters, values, dataContent);
            }
            else
            {
                throw new Exception($"不支持的数据格式。{dataType}");
            }
        }

        private static void ProcessingXmlData<T>(IEnumerable<T> parameters, ConcurrentDictionary<string, string> values, string content) where T : IValueParameter
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            Parallel.ForEach(parameters, (parameter) =>
             {
                 string key = parameter.Key;
                 var value = xmlDocument.DocumentElement.SelectSingleNode(parameter.ValueOrExpression).InnerText;
                 values.TryAdd(key, value);
                 if (parameter.DataTag.HasValue)
                 {
                     values.TryAdd(parameter.DataTag.ToString(), value);
                 }
             });
        }

        private static void ProcessingJsonData<T>(IEnumerable<T> parameters, ConcurrentDictionary<string, string> values, string content) where T : IValueParameter
        {
            var jObject = JObject.Parse(content);
            Parallel.ForEach(parameters, (parameter) =>
             {
                 string key = parameter.Key;
                 string value = jObject.SelectToken(parameter.ValueOrExpression).ToString();
                 values.TryAdd(key, value);
                 if (parameter.DataTag.HasValue)
                     values.TryAdd(parameter.DataTag.ToString(), value);
             });

        }


        [HttpPost]
        public async Task<IActionResult> QueryOrder(QueryOrderDto input)
        {
            var app = await QueryApp(input);
            var signContent = input.GetSignContent();
            if (VerifySign(input.Sign, signContent, app.Key))
            {

                return Content("验签成功");
            }
            else
            {
                return Content("验签失败");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CloseOrder(CloseOrderDto input)
        {
            var app = await QueryApp(input);
            var signContent = input.GetSignContent();
            if (VerifySign(input.Sign, signContent, app.Key))
            {

                return Content("验签成功");
            }
            else
            {
                return Content("验签失败");
            }
        }

        [Route("Pay/Callback_{CompanyId}_{ChannelId}_{AccountId}")]
        public async Task<IActionResult> Callback(int CompanyId, int ChannelId, int AccountId)
        {
            int tenantId = AbpSession.GetTenantId();
            //TODO 获取回调参数
            IFormCollection form = null;
            if (Request.HasFormContentType)
            {
                form = Request.Form;
            }
            string bodyContent = Request.GetBodyContent();
            IRequestCookieCollection cookies = Request.Cookies;
            IHeaderDictionary headers = Request.Headers;
            IQueryCollection query = Request.Query;
            var data = new ConcurrentDictionary<string, string>();
            var parameters = await orderProcessingService.GetOrderCallbackParametersAsync(CompanyId);
            //TODO DataType
            var dataType = DataType.FormData;
            var groups = parameters.ToLookup(p => p.GetLocation);
            IGrouping<GetValueLocation?, ApiParameter> parameterGroup = null;
            foreach (var g in groups)
            {
                switch (g.Key)
                {
                    case GetValueLocation.Form:
                        foreach (var parameter in g)
                        {
                            string key = parameter.Key;
                            if (form.ContainsKey(key))
                            {
                                string value = form[key];
                                data.TryAdd(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.TryAdd(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    case GetValueLocation.Body:
                        await ProcessingData(data, bodyContent, dataType, g.ToList());
                        break;
                    case GetValueLocation.Query:
                        foreach (var parameter in g)
                        {
                            string key = parameter.Key;
                            if (query.ContainsKey(key))
                            {
                                string value = query[key];
                                data.TryAdd(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.TryAdd(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    case GetValueLocation.Headers:
                        foreach (var parameter in g)
                        {
                            string key = parameter.Key;
                            if (headers.ContainsKey(key))
                            {
                                string value = headers[key];
                                data.TryAdd(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.TryAdd(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    //case GetValueLocation.Cookies:
                    //    foreach (var parameter in g)
                    //    {
                    //        string key = parameter.Key;
                    //        if (cookies.ContainsKey(key))
                    //        {
                    //            string value = cookies[key];
                    //            data.TryAdd(key, value);
                    //            if (parameter.DataTag.HasValue)
                    //            {
                    //                data.TryAdd(parameter.DataTag.Value.ToString(), value);
                    //            }
                    //        }
                    //    }
                    //    break;
                    case GetValueLocation.Parameter:
                        parameterGroup = g;
                        break;
                    default:
                        break;
                }
            }
            if (parameterGroup != null && parameterGroup.Count() > 0)
            {
                var variable = new PaymentVariable(null, null, null, null);
                variable.InitValues(new Dictionary<string, string>(data));
                var values = variable.ProcessingApiCallbackParameters(parameterGroup);
                foreach (var item in values)
                {
                    if (!data.ContainsKey(item.Key))
                        data.TryAdd(item.Key, item.Value);
                }
            }
            if (data == null || data.Count == 0)
            {
                throw new Exception("渠道未开通");
            }
            var orderNo = data[DataTag.SystemOrderId.ToString()];
            var resultCode = data[DataTag.ResultCode.ToString()];
            var resultCodeMean = await resultCodeRepository.FirstOrDefaultAsync(c => c.ResultCode == resultCode &&
            c.CompanyId == CompanyId);
            if (resultCodeMean.Mean == ResultCodeMean.Succeed)
            {
                if (data.ContainsKey(DataTag.Sign.ToString()) && data.ContainsKey(DataTag.GenSign.ToString()) && data[DataTag.Sign.ToString()] == data[DataTag.GenSign.ToString()])
                {
                    var result = await orderProcessingService.MarkOrderAsPaid(long.Parse(orderNo));
                    if (result.IsSuccess && result.Data == PaymentStatus.Paid)
                        return Content("Success");
                    else return Content("Failed");
                }
                else
                {
                    throw new Exception("验签失败");
                }
            }
            return View();
        }
    }
}
