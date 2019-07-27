using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using JK.Payments.Enumerates;

namespace JK.Payments.Tenants
{
    [Table("BalanceChangedRecords")]
    public class BalanceChangedRecord : Entity<long>, IHasCreationTime
    {
        public int TenantId { get; set; }

        public int CompanyId { get; set; }

        public int AccountId { get; set; }

        public int Amount { get; set; }

        public DateTime CreationTime { get; set; }

        public long RelationId { get; set; }

        public BalanceChangedReasonType Reason { get; set; }
    }
}