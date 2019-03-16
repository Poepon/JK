using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreatePrivateSessionInput
    {
        [Key("tguid")]
        public long TargetUserId { get; set; }
    }
}
