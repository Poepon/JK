using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    /// <summary>
    /// 群组
    /// </summary>
    [Table("ChatGroups")]
    public class ChatGroup : Entity<long>, ICreationAudited
    {
        public string Name { get; set; }

        public string Notice { get; set; }

        public ChatGroupType GroupType { get; set; }

        public long? CreatorUserId { get; set; }

        public DateTime CreationTime { get; set; }

        public int Status { get; set; }

    }
}
