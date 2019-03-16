namespace JK.Chat.Dto
{
    public class CreatePrivateSessionInput
    {
        public long CreatorUserId { get; set; }

        public long TargetUserId { get; set; }
        
    }
}
