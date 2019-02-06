namespace JK.Chat.Dto.Input
{
    public class UploadFileInput
    {
        public long GroupId { get; set; }

        public long UserId { get; set; }

        public FileType FileType { get; set; }

        public byte[] File { get; set; }
    }
}
