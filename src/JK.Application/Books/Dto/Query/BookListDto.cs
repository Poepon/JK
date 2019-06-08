using Abp.Application.Services.Dto;

namespace JK.Books.Dto
{
    public class BookListDto : EntityDto
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public bool UseLocalImage { get; set; }

        /// <summary>
        /// 作者姓名
        /// </summary>
        public string AuthorName { get; set; }

    }
}
