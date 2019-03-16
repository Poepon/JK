using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    public class SessionUnreadOutput
    {
        [Key("count")]
        public int Count { get; set; }
    }
}
