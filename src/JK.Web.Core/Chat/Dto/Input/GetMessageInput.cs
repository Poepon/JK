using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class GetMessageInput
    {
        [Key("gid")]
        public long GroupId { get; set; }

        [Key("mid")]
        public long MessageId { get; set; }

        [Key("d")]
        public QueryDirection Direction { get; set; }

        [Key("c")]
        public int TakeCount { get; set; }
    }
}
