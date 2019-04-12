using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreatePrivateSessionInput
    {
        [Key("tid")]
        public int? TargetTenantId { get; set; }

        [Key("tguid")]
        public long TargetUserId { get; set; }
    }
}
