namespace JK.Chat.Dto.Input
{
    public class ReadMessageInput
    {
        public long UserId { get; set; }

        public long GroupId { get; set; }

        public long MessageId { get; set; }
    }
}
