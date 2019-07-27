using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    /// <summary>
    /// API配置
    /// </summary>
    [Table("ApiConfigurations")]
    public class ApiConfiguration : Entity
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public ApiMethod ApiMethod { get; set; }

        public RequestType RequestType { get; set; }

        [Required]
        [StringLength(256)]
        public string Url { get; set; }

        [Required]
        [StringLength(10)]
        public string Method { get; set; }

        /// <summary>
        /// 发送给第三方的数据类型
        /// </summary>
        [Required]
        [StringLength(32)]
        public string ContentType { get; set; }

        /// <summary>
        /// 期望第三方返回的数据类型
        /// </summary>
        public Enumerates.DataType DataType { get; set; }

        [Required]
        [StringLength(16)]
        public string AcceptCharset { get; set; }

        public bool HasResponseParameter { get; set; }

        public virtual ICollection<ApiChannel> SupportedChannels { get; set; }

    }
}
