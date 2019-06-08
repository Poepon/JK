using Abp.Application.Services.Dto;

namespace JK.Books.Dto
{
    public class BookChapterDetailDto : EntityDto
    {
        public int PrePageChapterId { get; set; }

        public int NextPageChapterId { get; set; }

        public int BookId { get; set; }

        public string BookName { get; set; }

        public string Title { get; set; }

        public string AuthorName { get; set; }

        public bool HasSrc { get; set; }

        public string Src { get; set; }

        public bool IsFree { get; set; }

        public int Order { get; set; }
    }
}