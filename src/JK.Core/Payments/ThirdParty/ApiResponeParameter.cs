using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public interface IParameter
    {
        string Key { get; set; }

        string Expression { get; set; }

        DataTag? DataTag { get; set; }
    }
    public class ApiResponeParameter : Entity, IParameter
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        public string Key { get; set; }

        public string Expression { get; set; }

        public DataTag? DataTag { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
