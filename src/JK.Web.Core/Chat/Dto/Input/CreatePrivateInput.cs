using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreatePrivateInput
    {
        [Key("tguid")]
        public long TargetUserId { get; set; }
    }
}
