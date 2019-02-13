namespace JK.Chat.Dto
{
    public class SetLastReceivedIdInput : ChatGroupInputBase
    {
        public long LastReceivedMessageId { get; set; }
    }
}
