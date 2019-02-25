namespace JK.Chat.Dto
{
    public class GetGroupsUnreadOutput
    {
        public long GroupId { get; set; }
        public int Count { get; set; }
    }
    public class GetGroupsUnreadInput
    {
        public long UserId { get; set; }
    }
}
