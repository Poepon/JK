using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Alliance
{
    /// <summary>
    /// 代理关系
    /// </summary>
    [Table("AgentRelationships")]
    public class AgentRelationship : Entity<long>, IPassivable
    {
        public long AgentId { get; set; }

        public long SubAgentId { get; set; }

        public long SubParentId { get; set; }

        public int Level { get; set; }

        public decimal CommissionRate { get; set; }

        public decimal NodeCommissionRate { get; set; }

        public bool IsActive { get; set; }
    }
}
