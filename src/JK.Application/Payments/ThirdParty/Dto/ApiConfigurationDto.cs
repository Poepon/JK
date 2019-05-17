using Abp.Application.Services.Dto;
using JK.Payments.Enumerates;

namespace JK.Payments.ThirdParty.Dto
{
    public class ApiConfigurationDto : EntityDto
    {
        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        public RequestType RequestType { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public DataType DataType { get; set; }

        public string AcceptCharset { get; set; }


    }
}
