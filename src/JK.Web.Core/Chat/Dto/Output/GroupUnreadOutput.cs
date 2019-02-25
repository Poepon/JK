using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    public class GroupUnreadOutput
    {
        [Key("count")]
        public int Count { get; set; }
    }
}
