namespace JK.Chat.Dto
{
    public class SetLastReceivedIdInput : ChatSessionInputBase
    {
        public long LastReceivedMessageId { get; set; }
    }
}
