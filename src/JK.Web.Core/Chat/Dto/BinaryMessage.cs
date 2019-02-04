namespace JK.Chat.Dto
{
    public class BinaryMessage : IMessage<byte[]>
    {
        public int MessageType { get; set; }

        public int DataLength { get; set; }

        public byte[] Data { get; set; }
    }
}
