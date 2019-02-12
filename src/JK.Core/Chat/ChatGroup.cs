using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    /// <summary>
    /// 群组
    /// </summary>
    [Table("ChatGroups")]
    public class ChatGroup : Entity<long>, IHasCreationTime, IPassivable, IMayHaveTenant
    {
        public const int MaxNameLength = 100;
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(2048)]
        public string Description { get; set; }

        public ChatGroupType GroupType { get; set; }

        public long CreatorUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; } = true;

        public int? TenantId { get; set; }
    }
}
