﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Chat
{
    /// <summary>
    /// 聊天群组成员
    /// </summary>
    [Table("ChatGrouppMembers")]
    public class ChatGrouppMember : Entity<long>, IHasCreationTime
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public int Status { get; set; }

        public DateTime CreationTime { get; set; }
    }
}