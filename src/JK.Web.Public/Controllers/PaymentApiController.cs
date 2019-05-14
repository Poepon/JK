using Abp.AspNetCore.Mvc.Controllers;
using Abp.Domain.Repositories;
using JK.Cryptography;
using JK.Payments.Enumerates;
using JK.Payments.TenantConfigs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace JK.Web.Public.Controllers
{
    public abstract class PaymentApiDtoBase
    {
        public string AppId { get; set; }
    }
    public class PlaceOrderDto : PaymentApiDtoBase
    {

        public long Amount { get; set; }

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

        public string Sign { get; set; }

    }
    public class QueryOrderDto : PaymentApiDtoBase
    {

    }
    public class CloseOrderDto : PaymentApiDtoBase
    {

    }
    [Route("api/pay/[action]")]
    public class PaymentApiController : AbpController
    {
        private readonly IRepository<TenantApp> appRepository;

        public PaymentApiController(IRepository<TenantApp> appRepository)
        {
            this.appRepository = appRepository;
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
        protected bool VerifySign(PlaceOrderDto input, string key)
        {
            string stringSignTemp = $"{input.AppId}{input.ChannelCode}{input.BankCode}{input.ExternalOrderId}{input.Amount}{input.SyncCallback}{input.AsyncCallback}{input.ExtData}";
            return input.Sign == JKMd5.GetMd5(stringSignTemp + key);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(PlaceOrderDto input)
        {
            var app = await QueryApp(input);
            if (VerifySign(input, app.Key))
            {
                return Content("验签成功");
            }
            else
            {
                return Content("验签失败");
            }
        }

        [HttpPost]
        public async Task<IActionResult> QueryOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CloseOrder()
        {
            return View();
        }
    }
}
