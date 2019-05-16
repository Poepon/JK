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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly IRepository<ApiCallbackParameter> apiCallbackParameterRepository;
        private readonly IRepository<ResultCodeConfiguration> resultCodeRepository;
        public PaymentApiController(
            IHttpClientFactory httpClientFactory,
            IRepository<TenantApp> appRepository,
            IRepository<ResultCodeConfiguration> resultCodeRepository,
            IOrderProcessingService orderProcessingService,
            IRepository<ApiCallbackParameter> apiCallbackParameterRepository)
        {
            this.httpClientFactory = httpClientFactory;
            this.appRepository = appRepository;
            this.resultCodeRepository = resultCodeRepository;
            this.orderProcessingService = orderProcessingService;
            this.apiCallbackParameterRepository = apiCallbackParameterRepository;
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
                        var data = await ProcessingResponseData(response, postdata.DataType, postdata.ApiResponeParameters);
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

        private async Task<Dictionary<string, string>> ProcessingResponseData(HttpResponseMessage response, string dataType, List<ApiResponeParameter> responeParameters)
        {
            var values = new ConcurrentDictionary<string, string>();
            var content = await response.Content.ReadAsStringAsync();
            if (dataType == "json")
            {
                var jObject = JObject.Parse(content);
                Parallel.ForEach(responeParameters, (parameter) =>
                {
                    string key = parameter.Key;
                    string value = jObject.SelectToken(parameter.Value).ToString();
                    values.TryAdd(key, value);
                    if (parameter.DataTag.HasValue)
                        values.TryAdd(parameter.DataTag.ToString(), value);
                });
            }
            else if (dataType == "xml")
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content);
                Parallel.ForEach(responeParameters, (parameter) =>
                {
                    string key = parameter.Key;
                    var value = xmlDocument.DocumentElement.SelectSingleNode(parameter.Value).InnerText;
                    values.TryAdd(key, value);
                    if (parameter.DataTag.HasValue)
                    {
                        values.TryAdd(parameter.DataTag.ToString(), value);
                    }
                });
            }
            return new Dictionary<string, string>(values);
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
        public IActionResult Callback(int CompanyId, int ChannelId, int AccountId)
        {
            int tenantId = AbpSession.GetTenantId();
            //TODO 获取回调参数
            
            return View();
        }
    }
}
