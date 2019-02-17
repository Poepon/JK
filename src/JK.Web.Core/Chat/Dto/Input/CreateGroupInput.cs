using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreatePrivateInput
    {
        [Key("tguid")]
        public long TargetUserId { get; set; }
    }
    [MessagePackObject]
    public class CreateGroupInput
    {
        [Key("gn")]
        public string GroupName { get; set; }

        [Key("gt")]
        public ChatGroupType GroupType { get; set; }
    }
}
