using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using JK.Controllers;
using Abp.RabbitMQ.AutoSubscribe;
using System;

namespace JK.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : JKControllerBase
    {
        private readonly IRabbitMQProducer rabbitMQProducer;

        public HomeController(IRabbitMQProducer rabbitMQProducer)
        {
            this.rabbitMQProducer = rabbitMQProducer;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TestExceptionless()
        {
            throw new System.Exception("Test Exceptionless.");
        }

        public ActionResult TestRabbit()
        {
            var task = rabbitMQProducer.PublishAsync(new Volo.Abp.RabbitMQ.ExchangeDeclareConfiguration(typeof(string).FullName, "direct", false, true), "#", "hi");
            task.Wait();
            Console.WriteLine(2);
            return RedirectToAction("Index");
        }
    }
}
