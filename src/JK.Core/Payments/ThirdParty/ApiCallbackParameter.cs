using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{   
    public class ApiCallbackParameter : Entity, IGetValueParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Expression { get; set; }

        public bool Required { get; set; }

        public GetValueLocation Location { get; set; }

        public DataTag? DataTag { get; set; }

        public string Format { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }

      
    }
}
