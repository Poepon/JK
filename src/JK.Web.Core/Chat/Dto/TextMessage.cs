namespace JK.Chat.Dto
{
    public class TextMessage : IMessage<string>
    {
        public MessageType MessageType { get; set; }

        public MessageDataType DataType { get; set; }

        public string Data { get; set; }
    }
}
