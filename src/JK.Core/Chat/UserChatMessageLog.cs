using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    [Table("UserChatMessageLogs")]
    public class UserChatMessageLog : Entity<long>
    {
        public long GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual ChatGroup ChatGroup { get; set; }

        public long UserId { get; set; }

        public long LastReceivedMessageId { get; set; }

        public long LastReadMessageId { get; set; }
    }
}
