using MessagePack;

namespace JK.Chat.Distributed
{
    [MessagePackObject]
    public class ChatQueueDto
    {
        [Key("cid")]
        public string ConnectionId { get; set; }

        [Key("data")]
        public byte[] Data { get; set; }
    }
}