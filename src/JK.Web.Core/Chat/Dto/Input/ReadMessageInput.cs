namespace JK.Chat.Dto.Input
{
    public class ReadMessageInput
    {
        public long UserId { get; set; }

        public long SessionId { get; set; }

        public long MessageId { get; set; }
    }
}
