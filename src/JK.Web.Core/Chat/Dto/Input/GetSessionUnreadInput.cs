using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class GetSessionUnreadInput
    {
        [Key("gid")]
        public long SessionId { get; set; }
    }
}
