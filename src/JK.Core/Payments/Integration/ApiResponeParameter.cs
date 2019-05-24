using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    [Table("ApiResponeParameters")]
    public class ApiResponeParameter : Entity, IGetValueParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        [StringLength(32)]
        public string Key { get; set; }

        [Required]
        [StringLength(500)]
        public string Expression { get; set; }

        public DataTag? DataTag { get; set; }

        public GetValueLocation Location { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
