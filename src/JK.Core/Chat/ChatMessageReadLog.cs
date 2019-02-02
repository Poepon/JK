using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities;

namespace JK.Chat
{
    [Table("ChatMessageReadLogs")]
    public class ChatMessageReadLog : Entity<long>
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public long LastMessageId { get; set; }
    }
}
