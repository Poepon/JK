namespace JK.Chat.Dto
{
    public interface IMessage<T>
    {
        CommandType CommandType { get; set; }

        T Data { get; set; }
    }
}
