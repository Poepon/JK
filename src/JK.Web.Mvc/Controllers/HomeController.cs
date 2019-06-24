using Abp.AspNetCore.Mvc.Authorization;
using JK.Abp.RabbitMQ;
using JK.Abp.RabbitMQ.AutoSubscribe;
using JK.Controllers;
using Microsoft.AspNetCore.Mvc;
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
            var task = rabbitMQProducer.PublishAsync(new ExchangeDeclareConfiguration(typeof(string).FullName, "direct", false, true), "#", "hi");
            task.Wait();
            Console.WriteLine(2);
            return RedirectToAction("Index");
        }
    }
}
