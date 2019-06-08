using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace JK.Books.Dto
{
    [AutoMap(typeof(BookVolume))]
    public class BookVolumeDto : EntityDto
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public int Order { get; set; }

        public long QiDianVolumeId { get; set; }

        public bool IsVip { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; } = true;
    }
}