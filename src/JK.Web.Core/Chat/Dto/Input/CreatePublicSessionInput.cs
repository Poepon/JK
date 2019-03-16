using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreatePublicSessionInput
    {
        [Key("gname")]
        public string SessionName { get; set; }        
    }
}
