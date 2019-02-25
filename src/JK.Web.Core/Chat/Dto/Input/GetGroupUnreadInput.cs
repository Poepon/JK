using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class GetGroupUnreadInput
    {
        [Key("gid")]
        public long GroupId { get; set; }
    }
}
