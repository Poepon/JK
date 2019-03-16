using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Chat
{
    /// <summary>
    /// 会话成员
    /// </summary>
    [Table("ChatSessionMembers")]
    public class ChatSessionMember : Entity<long>, IPassivable
    {
        public long SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual ChatSession ChatSession { get; set; }

        public long UserId { get; set; }

        public long CreationTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
