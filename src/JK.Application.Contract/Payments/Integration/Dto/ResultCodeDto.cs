using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration.Dto
{
    [AutoMap(typeof(ResultCode))]
    public class ResultCodeDto : EntityDto
    {
        public int CompanyId { get; set; }

        [Required]
        [StringLength(16)]
        public string Code { get; set; }

        public ResultCodeMean Mean { get; set; }
    }
}
