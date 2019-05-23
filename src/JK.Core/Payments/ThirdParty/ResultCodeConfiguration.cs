using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// 响应结果代码配置
    /// </summary>
    public class ResultCodeConfiguration : Entity
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        [Required]
        [StringLength(16)]
        public string ResultCode { get; set; }

        public ResultCodeMean Mean { get; set; }

    }
}
