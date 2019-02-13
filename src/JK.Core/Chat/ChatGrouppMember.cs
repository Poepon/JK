using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    /// <summary>
    /// 聊天群组成员
    /// </summary>
    [Table("ChatGrouppMembers")]
    public class ChatGroupMember : Entity<long>, IHasCreationTime, IPassivable
    {
        public long GroupId { get; set; }
        
        [ForeignKey(nameof(GroupId))]
        public virtual ChatGroup ChatGroup { get; set; }

        public long UserId { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
