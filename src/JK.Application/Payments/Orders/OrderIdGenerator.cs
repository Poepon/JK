using System;
using System.Collections.Generic;
using Abp.Dependency;

namespace JK.Payments.Orders
{
    public class OrderIdGenerator : ISingletonDependency
    {
       
        public string OrderNoPrefix { get; set; }
     
        /// <summary>
        /// 生成订单ID
        /// </summary>
        /// <returns></returns>
        public string GenerateOrderId()
        {
            var tid = GenerateTempId();

            return OrderNoPrefix + DateTime.Now.ToString("yyMMddHHmm") + tid;
        }

        private string GenerateTempId()
        {
            int hashCode = Guid.NewGuid().GetHashCode();
            if (hashCode < 0)
            {
                hashCode = -hashCode;
            }
            return string.Format("{0,10:D10}", hashCode) ;
        }
    }
}
