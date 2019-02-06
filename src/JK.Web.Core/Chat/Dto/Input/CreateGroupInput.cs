namespace JK.Chat.Dto.Input
{
    public class CreateGroupInput
    {
        public string GroupName { get; set; }

        public long UserId { get; set; }

        public ChatGroupType GroupType { get; set; }
    }
}
