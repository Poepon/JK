namespace JK.Chat.Dto
{
    public class SetLastReadIdInput : ChatGroupInputBase
    {
        public long LastReadMessageId { get; set; }
    }

}
