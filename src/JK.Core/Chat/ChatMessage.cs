using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    [Table("ChatMessages")]
    public class ChatMessage : Entity<long>, IMayHaveTenant, IHasCreationTime
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        public long UserId { get; set; }

        public long GroupId { get; set; }

        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        public int? TenantId { get; set; }

        public DateTime CreationTime { get; set; }

        public ChatMessageReadState ReadState { get; set; }
    }
}
