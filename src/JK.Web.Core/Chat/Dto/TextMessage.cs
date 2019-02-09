namespace JK.Chat.Dto
{
    public class TextMessage : IMessage<string>
    {
        public CommandType CommandType { get; set; }

        public MessageDataType DataType { get; set; }

        public string Data { get; set; }
    }
}
