using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using JK.Cryptography;
using JK.Payments;
using JK.Payments.Enumerates;
using JK.Payments.Orders;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Payments.ThirdParty;
using JK.Web.Public.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace JK.Web.Public.Controllers
{
    [Route("api/pay/[action]")]
    public class PaymentApiController : AbpController
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepository<TenantApp> appRepository;
        private readonly IOrderProcessingService orderProcessingService;
        private readonly IRepository<ResultCodeConfiguration> resultCodeRepository;
        public PaymentApiController(
            IHttpClientFactory httpClientFactory,
            IRepository<TenantApp> appRepository,
            IRepository<ResultCodeConfiguration> resultCodeRepository,
            IOrderProcessingService orderProcessingService)
        {
            this.httpClientFactory = httpClientFactory;
            this.appRepository = appRepository;
            this.resultCodeRepository = resultCodeRepository;
            this.orderProcessingService = orderProcessingService;
        }

        protected Task<TenantApp> QueryApp(PaymentApiDtoBase input)
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
            return sign == JKMd5.GetMd5(signContent + key);
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

            if (postdata.RequestType == RequestType.WebPage)
            {
                if (postdata.Method == "post")
                {
                    return View(postdata);
                }
                else
                {
                    var httpUrl = QueryHelpers.AddQueryString(postdata.Url, postdata.RequestData);
                    return Redirect(httpUrl);
                }
            }
            else
            {
                if (postdata.Method == "get")
                {
                    postdata.Url = QueryHelpers.AddQueryString(postdata.Url, postdata.RequestData);
                }

                var httpClient = httpClientFactory.CreateClient();
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(
                  postdata.Method == "post" ?
                    HttpMethod.Post :
                    HttpMethod.Get, postdata.Url);
                if (postdata.Method == "post")
                {
                    httpRequestMessage.Content = new FormUrlEncodedContent(postdata.RequestData);
                }

                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    if (postdata.HasResponeParameter)
                    {
                        var data = await ProcessingData(await response.Content.ReadAsStringAsync(), postdata.DataType, postdata.ApiResponeParameters);
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

        private CreatePaymentOrderDto BuildOrder(PlaceOrderDto input, TenantApp app)
        {
            return new CreatePaymentOrderDto
            {
                TenantId = app.TenantId,
                ChannelCode = input.ChannelCode,
                Amount = input.Amount,
                BankCode = input.BankCode,
                CreateIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                Device = app.Device,
                ExternalOrderId = input.ExternalOrderId,
                SyncCallback = input.SyncCallback,
                AsyncCallback = input.AsyncCallback,
                ExtData = input.ExtData
            };
        }

        private async Task<Dictionary<string, string>> ProcessingData<T>(string dataContent, DataType dataType, List<T> responeParameters) where T : IParameter
        {
            var values = new ConcurrentDictionary<string, string>();
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
            return new Dictionary<string, string>(values);
        }

        private static void ProcessingXmlData<T>(List<T> parameters, ConcurrentDictionary<string, string> values, string content) where T : IParameter
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);
            parameters.ForEach((parameter) =>
            {
                string key = parameter.Key;
                var value = xmlDocument.DocumentElement.SelectSingleNode(parameter.Expression).InnerText;
                values.TryAdd(key, value);
                if (parameter.DataTag.HasValue)
                {
                    values.TryAdd(parameter.DataTag.ToString(), value);
                }
            });
        }

        private static void ProcessingJsonData<T>(List<T> parameters, ConcurrentDictionary<string, string> values, string content) where T : IParameter
        {
            var jObject = JObject.Parse(content);
            parameters.ForEach((parameter) =>
            {
                string key = parameter.Key;
                string value = jObject.SelectToken(parameter.Expression).ToString();
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
            var data = new Dictionary<string, string>();
            var parameters = await orderProcessingService.GetOrderCallbackParametersAsync(CompanyId);
            var dataType = DataType.FormData;
            var groups = parameters.GroupBy(p => p.Location);
            List<ApiCallbackParameter> parameterGroup = null;
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
                                data.Add(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.Add(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    case GetValueLocation.Body:
                        await ProcessingData(bodyContent, dataType, g.ToList());
                        break;
                    case GetValueLocation.Query:
                        foreach (var parameter in g)
                        {
                            string key = parameter.Key;
                            if (query.ContainsKey(key))
                            {
                                string value = query[key];
                                data.Add(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.Add(parameter.DataTag.Value.ToString(), value);
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
                                data.Add(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.Add(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    case GetValueLocation.Cookies:
                        foreach (var parameter in g)
                        {
                            string key = parameter.Key;
                            if (cookies.ContainsKey(key))
                            {
                                string value = cookies[key];
                                data.Add(key, value);
                                if (parameter.DataTag.HasValue)
                                {
                                    data.Add(parameter.DataTag.Value.ToString(), value);
                                }
                            }
                        }
                        break;
                    case GetValueLocation.Parameter:
                        parameterGroup = g.ToList();
                        break;
                    default:
                        break;
                }
            }

            return View();
        }
    }
}
