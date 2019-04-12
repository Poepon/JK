using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Chat
{
    /// <summary>
    /// 会话
    /// </summary>
    [Table("ChatSessions")]
    public class ChatSession : Entity<long>, IPassivable
    {
        public const int MaxNameLength = 100;
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [StringLength(2048)]
        public string Description { get; set; }

        public ChatSessionType SessionType { get; set; }

        public int? CreatorTenantId { get; set; }

        public long CreatorUserId { get; set; }

        public long CreationTime { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
