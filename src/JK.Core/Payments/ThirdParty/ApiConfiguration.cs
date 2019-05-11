using JK.Payments.Enumerates;
using System;
namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// API配置
    /// </summary>
    public class ApiConfiguration
    {
        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        public RequestType RequestType { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public string DataType { get; set; }

        public string AcceptCharset { get; set; }


    }
}
