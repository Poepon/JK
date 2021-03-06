﻿using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Chat
{
    [Table("UserChatMessageLogs")]
    public class UserChatMessageLog : Entity<long>
    {
        public long SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual ChatSession ChatSession { get; set; }

        public long UserId { get; set; }

        public long LastReceivedMessageId { get; set; }

        public long LastReadMessageId { get; set; }
    }
}
