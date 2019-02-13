using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using JK.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    [Table("ChatMessages")]
    public class ChatMessage : Entity<long>, IHasCreationTime
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        public long UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual ChatGroup ChatGroup { get; set; }

        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        public DateTime CreationTime { get; set; }

        public ChatMessageReadState ReadState { get; set; }
    }
}
