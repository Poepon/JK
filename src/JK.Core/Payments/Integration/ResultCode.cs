using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    /// <summary>
    /// 响应结果代码配置
    /// </summary>
    [Table("ResultCode")]
    public class ResultCode : Entity
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        [Required]
        [StringLength(16)]
        public string Code { get; set; }

        public ResultCodeMean Mean { get; set; }

    }
}
