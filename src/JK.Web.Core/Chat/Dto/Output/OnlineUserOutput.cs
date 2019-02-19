using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    public class OnlineUserOutput
    {
        [Key("uid")]
        public long UserId { get; set; }

        [Key("uname")]
        public string UserName { get; set; }

        [Key("icon")]
        public string Icon { get; set; }
    }
}
