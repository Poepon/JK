namespace JK.Chat.Dto
{
    public class JsonMessage : IMessage<string>
    {
        public MessageType MessageType { get; set; }

        public string Data { get; set; }
    }
}
