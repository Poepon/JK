namespace JK.Chat.Dto
{
    public class CreatePrivateSessionInput
    {
        public int? CreatorTenantId { get; set; }

        public long CreatorUserId { get; set; }

        public int? TargetTenantId { get; set; }

        public long TargetUserId { get; set; }
        
    }
}
