using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{   
    public class ApiRequestParameter : Entity, ISetValueParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        [StringLength(32)]
        public string Key { get; set; }

        [Required]
        [StringLength(500)]
        public string ValueOrExpression { get; set; }

        public SetValueLocation Location { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }

    }
}
