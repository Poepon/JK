﻿using Abp.AutoMapper;
using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    [AutoMapFrom(typeof(ChatSessionDto))]
    public class ChatSessionOutput
    {
        [Key("gid")]
        public long Id { get; set; }

        [Key("gname")]
        public string Name { get; set; }

        [Key("icon")]
        public string Icon { get; set; }

        [Key("gt")]
        public ChatSessionType SessionType { get; set; }

        [Key("owner")]
        public long CreatorUserId { get; set; }

        [Key("time")]
        public long CreationTime { get; set; }

        [Key("s")]
        public int Status { get; set; }

        [Key("unread")]
        public int Unread { get; set; }

        [Key("cur")]
        public bool IsCurrent { get; set; }

    }
}
