using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using JK.Cryptography;
using JK.Payments.Enumerates;
using JK.Payments.Orders;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs;
using JK.Web.Public.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JK.Web.Public.Controllers
{

    [Route("api/pay/[action]")]
    public class PaymentApiController : AbpController
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepository<TenantApp> appRepository;
        private readonly IOrderProcessingService orderProcessingService;

        public PaymentApiController(
            IHttpClientFactory httpClientFactory,
            IRepository<TenantApp> appRepository,
            IOrderProcessingService orderProcessingService)
        {
            this.httpClientFactory = httpClientFactory;
            this.appRepository = appRepository;
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
                    var httpUrl = QueryHelpers.AddQueryString(postdata.Url, postdata.Parameters);
                    return Redirect(httpUrl);
                }
            }
            else
            {
                if (postdata.Method == "get")
                {
                    postdata.Url = QueryHelpers.AddQueryString(postdata.Url, postdata.Parameters);
                }

                var httpClient = httpClientFactory.CreateClient();
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(
                  postdata.Method == "post" ?
                    HttpMethod.Post :
                    HttpMethod.Get, postdata.Url);
                if (postdata.Method == "post")
                {
                    //if (interfaceConfig.ContentType == ContentType.Json)
                    //{
                    //    httpRequestMessage.Content = new StringContent(dic.ToJsonString(), Encoding.UTF8, "application/json");
                    //}
                    //else if (interfaceConfig.ContentType == ContentType.Xml) {

                    //}
                    //else
                    //{
                    httpRequestMessage.Content = new FormUrlEncodedContent(postdata.Parameters);
                    //  }
                }

                var response = await httpClient.SendAsync(httpRequestMessage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();

                    var data = await ProcessingResponseData(response);
                    //TODO 
                    return Content("");
                }
                else
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

        private async Task<Dictionary<string, string>> ProcessingResponseData(HttpResponseMessage response)
        {
            var values = new Dictionary<string, string>();
            var _json = await response.Content.ReadAsStringAsync();

            return values;

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
    }
}
