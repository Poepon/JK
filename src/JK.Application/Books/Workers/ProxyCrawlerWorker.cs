using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace JK.Books.Crawlers
{
    public static class StaticProxys
    {
        public static bool UseProxy = false;
        public static ConcurrentQueue<ProxyModel> ProxyModels { get; set; } = new ConcurrentQueue<ProxyModel>();
    }
    /// <summary>
    /// 匿名代理爬虫
    /// </summary>
    public class ProxyCrawlerWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly HttpClient _httpClient;
        private const int CheckPeriodAsMilliseconds = 2 * 60 * 1000; //2分钟
        private readonly string url = "http://www.xicidaili.com/nn/";
        public ProxyCrawlerWorker(AbpTimer timer) : base(timer)
        {
            _httpClient = new HttpClient();
            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        protected override void DoWork()
        {
            Console.WriteLine("爬代理IP开始。");
            try
            {
                var response = _httpClient.GetString(url,Encoding.UTF8);
                var doc = new HtmlDocument();
                doc.LoadHtml(response.html);
                var node = doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]");
                var trs = node.SelectNodes("tr");
                System.Threading.Tasks.Parallel.ForEach(trs, (tr) =>
                {
                    var country = tr.SelectSingleNode("td[1]/img");
                    if (country != null)
                    {
                        var model = new ProxyModel
                        {
                            Country = country.GetAttributeValue("alt", ""),
                            IpAddress = tr.SelectSingleNode("td[2]").InnerText,
                            Port = int.Parse(tr.SelectSingleNode("td[3]").InnerText),
                            ProxyType = tr.SelectSingleNode("td[5]").InnerText,
                            Scheme = tr.SelectSingleNode("td[6]").InnerText,
                            UpDateTime = tr.SelectSingleNode("td[10]").InnerText
                        };
                        if (model.Validate())
                        {
                            StaticProxys.ProxyModels.Enqueue(model);
                        }
                    }
                });
            }
            catch
            {

            }
            finally
            {
                Console.WriteLine("爬代理IP结束。");
            }
        }
    }

    public class ProxyModel
    {
        public string Country { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string ProxyType { get; set; }

        public string Scheme { get; set; }

        public string UpDateTime { get; set; }

        public bool Validate()
        {
            string url = "http://ip-api.com/json";
            var handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
                Proxy = new WebProxy(IpAddress, Port),
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls | SslProtocols.None,
                UseProxy = true,
                UseCookies = true
            };
            using (HttpClient httpClient = new HttpClient(handler))
            {
                try
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(2);
                    var respone = httpClient.GetString(url,Encoding.UTF8);
                    var ipInfo = JsonConvert.DeserializeObject<IpModel>(respone.html);
                    Console.WriteLine("验证成功");
                    return ipInfo.query == IpAddress;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public class IpModel
        {
            public string status { get; set; }

            public string country { get; set; }
            public string countryCode { get; set; }
            public string region { get; set; }
            public string regionName { get; set; }
            public string city { get; set; }
            public string zip { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public string timezone { get; set; }
            public string isp { get; set; }
            public string org { get; set; }
            public string @as { get; set; }
            public string query { get; set; }
        }
    }
}