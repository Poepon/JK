namespace JK.Chat.Dto
{
    public class JsonMessage : IMessage<string>
    {
        public int MessageType { get; set; }

        public string Data { get; set; }
    }
}
