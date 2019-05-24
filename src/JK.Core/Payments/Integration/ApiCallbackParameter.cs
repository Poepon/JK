using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{   
    [Table("ApiCallbackParameters")]
    public class ApiCallbackParameter : Entity, IGetValueParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [StringLength(32)]
        [Required]
        public string Key { get; set; }

        [StringLength(500)]
        [Required]
        public string Expression { get; set; }

        public bool Required { get; set; }

        public GetValueLocation Location { get; set; }

        public DataTag? DataTag { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }

      
    }
}
