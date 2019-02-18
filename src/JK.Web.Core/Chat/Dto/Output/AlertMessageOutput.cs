using MessagePack;

namespace JK.Chat.Dto.Output
{
    [MessagePackObject]
    public class AlertMessageOutput
    {
        [Key("text")]
        public string Text { get; set; }

        [Key("type")]
        public AlertType Type { get; set; }

        [Key("title")]
        public string Title { get; set; }
        
    }
}
