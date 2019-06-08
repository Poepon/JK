using Abp.Domain.Repositories;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.IRepositories;
using System.Collections.Generic;
using System.Linq;

namespace JK.Books.Implementation
{
    public class BookVolumeAppService : JKAppServiceBase, IBookVolumeAppService
    {
        private readonly IBookVolumeRepository _volumeRepository;

        public BookVolumeAppService(IBookVolumeRepository volumeRepository)
        {
            _volumeRepository = volumeRepository;
        }


        public List<BookVolumeListDto> GetBookVolumes(int bookId)
        {
            var items = _volumeRepository.GetAll()
                .Where(v => v.BookId == bookId && v.IsActive)
                .OrderBy(v => v.Order)
                .Select(v => new BookVolumeListDto()
                {
                    Id = v.Id,
                    Name = v.Name
                }).ToList();
            return items;
        }
    }
}