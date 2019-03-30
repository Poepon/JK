using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.MultiTenancy
{
    [Table("TenantDomains")]
    public class TenantDomain : Entity
    {
        [StringLength(500)]
        public string Host { get; set; }

        [Range(0, 65535)]
        public int Port { get; set; } = 80;

        public int TenantId { get; set; }
    }
}