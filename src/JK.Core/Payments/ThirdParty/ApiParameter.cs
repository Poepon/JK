using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public class ApiParameter : Entity
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        public string Key { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }

        public string Value { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        public string Format { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
