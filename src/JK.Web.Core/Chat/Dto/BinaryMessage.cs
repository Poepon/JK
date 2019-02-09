namespace JK.Chat.Dto
{
    public class BinaryMessage : IMessage<byte[]>
    {
        public CommandType CommandType { get; set; }

        public MessageDataType DataType { get; set; }

        public byte[] Data { get; set; }
    }
}
