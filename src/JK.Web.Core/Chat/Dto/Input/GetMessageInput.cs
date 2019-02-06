namespace JK.Chat.Dto.Input
{
    public class GetMessageInput
    {
        public long UserId { get; set; }

        public long GroupId { get; set; }

        public long MessageId { get; set; }

        public QueryDirection Direction { get; set; }

        public int TakeCount { get; set; }
    }
}
