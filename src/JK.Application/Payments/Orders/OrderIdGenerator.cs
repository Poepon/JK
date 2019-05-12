using System;
using Abp.Dependency;

namespace JK.Payments.Orders
{
    public class OrderIdGenerator : ISingletonDependency
    {
        private readonly AbpMemoryCacheManager _memoryCacheManager;
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<int, List<int>> memoryCache;
        public OrderIdGenerator(AbpMemoryCacheManager memoryCacheManager, ICacheManager cacheManager)
        {
            _memoryCacheManager = memoryCacheManager;
            _cacheManager = cacheManager;
            memoryCache = _memoryCacheManager.GetCache<int, List<int>>("PaymentOrderId");
            memoryCache.DefaultSlidingExpireTime = TimeSpan.FromMilliseconds(10);
        }
        private int? serverId;
        public int GetServerId()
        {
            if (!serverId.HasValue)
            {
                var servername = Environment.MachineName;
                var cache = _cacheManager.GetCache<string, List<string>>("WebServers");
                var list = cache.GetOrDefault("All");
                if (list == null)
                {
                    list = new List<string>();
                    cache.Set("All", list);
                }
                if (!list.Contains(servername))
                {
                    list.Add(servername);
                    cache.Set("All", list);
                }
                serverId = list.FindIndex(i => i == servername) + 1;
            }
            return serverId.GetValueOrDefault();
        }
        /// <summary>
        /// 生成订单ID
        /// </summary>
        /// <returns></returns>
        public string GenerateOrderId()
        {
            var tid = GenerateTempId();

            return GetServerId() + Clock.Now.ToString("yyMMddHHmmssffff") + tid;
        }

        private int GenerateTempId()
        {
            Random random = new Random();
            int tid = random.Next(10, 100);

            var key = Clock.Now.Millisecond;
            var vs = memoryCache.GetOrDefault(key);
            if (vs == null)
                vs = new List<int>();
            if (vs.Count >= 90)
            {
                Thread.Sleep(1);
                vs = new List<int>();
            }
            while (IsExists(vs, tid))
            {
                tid = random.Next(10, 100);
            }
            vs.Add(tid);
            memoryCache.Set(key, vs);
            return tid;
        }

        private bool IsExists(List<int> vs, int tid)
        {
            return vs != null && vs.Contains(tid);
        }

    }
}
