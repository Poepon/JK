using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration.Dto.ApiParameters
{
    [AutoMap(typeof(ApiParameter))]
    public class ApiParameterDto : EntityDto
    {
        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        [StringLength(32)]
        [Required]
        public string Key { get; set; }

        [StringLength(500)]
        [Required]
        public string ValueOrExpression { get; set; }

        public bool Required { get; set; }

        public ParameterType ParameterType { get; set; }

        public DataTag? DataTag { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        public Location? Location { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
