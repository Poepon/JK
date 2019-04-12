namespace JK.Chat.Dto
{
    public class CreatePublicSessionInput
    {
        public string SessionName { get; set; }

        public int? CreatorTenantId { get; set; }

        public long CreatorUserId { get; set; }
    }
}
