using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
  
    [Table("ApiParameters")]
    public class ApiParameter : Entity, IValueParameter
    {

        public int CompanyId { get; set; }

        public ApiMethod ApiMethod { get; set; }

        [StringLength(32)]
        [Required]
        public string Key { get; set; }

        [StringLength(500)]
        [Required]
        public string ValueOrExpression { get; set; }

        public bool Required { get; set; }

        public ParameterType ParameterType { get; set; }

        public DataTag? DataTag { get; set; }

        [StringLength(32)]
        public string Format { get; set; }

        public GetValueLocation? GetLocation { get; set; }

        public SetValueLocation? SetLocation { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        [StringLength(100)]
        public string EncryptionParameters { get; set; }

        [StringLength(32)]
        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}