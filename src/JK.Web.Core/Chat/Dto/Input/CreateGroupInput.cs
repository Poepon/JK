using MessagePack;

namespace JK.Chat.Dto.Input
{
    [MessagePackObject]
    public class CreateGroupInput
    {
        [Key("gn")]
        public string GroupName { get; set; }        
    }
}
