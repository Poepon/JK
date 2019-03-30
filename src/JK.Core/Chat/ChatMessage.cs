using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JK.Authorization.Users;

namespace JK.Chat
{
    [Table("ChatMessages")]
    public class ChatMessage : Entity<long>
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        public long UserId { get; set; }

        [StringLength(20)]
        public string SenderName { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public long SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual ChatSession ChatSession { get; set; }

        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        public long CreationTime { get; set; }

        public ChatMessageReadState ReadState { get; set; }
    }
}
