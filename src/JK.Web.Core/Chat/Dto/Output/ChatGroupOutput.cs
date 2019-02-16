using Abp.AutoMapper;
using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    [AutoMapFrom(typeof(ChatGroupDto))]
    public class ChatGroupOutput
    {
        [Key("gid")]
        public long Id { get; set; }

        [Key("gn")]
        public string Name { get; set; }

        [Key("ico")]
        public string Icon { get; set; }

        [Key("gt")]
        public ChatGroupType GroupType { get; set; }

        [Key("owner")]
        public long CreatorUserId { get; set; }

        [Key("time")]
        public long CreationTime { get; set; }

        [Key("s")]
        public int Status { get; set; }

        [Key("lstmsg")]
        public string LastMessage { get; set; }

        [Key("lsttime")]
        public long LastTime { get; set; }

        [Key("unread")]
        public int Unread { get; set; }

        [Key("cur")]
        public bool IsCurrent { get; set; }
    }
}
