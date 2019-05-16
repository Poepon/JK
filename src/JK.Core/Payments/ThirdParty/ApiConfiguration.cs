using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System;
namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// API配置
    /// </summary>
    public class ApiConfiguration : Entity
    {
        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        public RequestType RequestType { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        /// <summary>
        /// 发送给第三方的数据类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 期望第三方返回的数据类型
        /// </summary>
        public string DataType { get; set; }

        public string AcceptCharset { get; set; }

        public bool HasResponeParameter { get; set; }
    }
}
