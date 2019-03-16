namespace JK.Chat.Dto
{
    public class SetLastReadIdInput : ChatSessionInputBase
    {
        public long LastReadMessageId { get; set; }
    }

}
