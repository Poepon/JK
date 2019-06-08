using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMapFrom(typeof(BookVolume))]
    public class BookVolumeListDto : EntityDto
    {
        public string Name { get; set; }

        [AutoMapper.IgnoreMap]
        public virtual IReadOnlyList<BookChapterListDto> Chapters { get; set; }
    }
}