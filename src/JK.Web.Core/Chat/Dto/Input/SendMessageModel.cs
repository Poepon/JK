using Abp.AutoMapper;
using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    [AutoMap(typeof(SendMessageInput))]
    public class SendMessageModel
    {
        [Key("gid")]
        public long GroupId { get; set; }

        [Key("msg")]
        public string Message { get; set; }
    }
}
