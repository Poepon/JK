namespace JK.Chat.Dto
{
    public class ChatSessionInputBase
    {
        public int? TenantId { get; set; }

        public long UserId { get; set; }

        public long SessionId { get; set; }
    }
}
