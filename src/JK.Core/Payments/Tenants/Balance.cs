using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Payments.Tenants
{
    [Table("Balances")]
    public class Balance : Entity
    {
        public int TenantId { get; set; }

        public int CompanyId { get; set; }

        public int AccountId { get; set; }

        public long Amount { get; set; }
    }
}