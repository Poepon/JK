using Abp.Domain.Entities;
using JK.Payments.Enumerates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public class ApiResponeParameter : Entity
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        [Required]
        public string Key { get; set; }

        public string Value { get; set; }

        public DataTag? DataTag { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
