namespace JK.Chat.Dto
{
    public class SendMessageInput : ChatSessionInputBase
    {
        public string Message { get; set; }

        public string SenderName { get; set; }
    }
    
}
