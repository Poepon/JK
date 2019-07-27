using Abp.Application.Services.Dto;

namespace JK.Payments.Integration.Dto.ApiParameters
{
    public class ApiParameterListDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string ApiMethod { get; set; }

        public string Key { get; set; }

        public string ParameterType { get; set; }

        public string Desc { get; set; }
    }
}