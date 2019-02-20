using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreateGroupInput
    {
        [Key("gname")]
        public string GroupName { get; set; }        
    }
}
