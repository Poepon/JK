namespace JK.Chat.Dto
{
    public enum MessageDataType : byte
    {
        Text = 0,
        Json = 1,
        MessagePack = 2,
        Protobuf = 3,
        Blob = 4
    }
}
