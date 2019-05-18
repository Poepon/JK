using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public class ApiResponeParameter : Entity, IParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        public string Key { get; set; }

        public ExpressionType ExpType { get; set; }

        public string Expression { get; set; }

        public DataTag? DataTag { get; set; }

        public GetValueLocation Location { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
