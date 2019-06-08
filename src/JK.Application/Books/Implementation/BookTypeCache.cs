using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.IRepositories;
using System.Collections.Generic;
using System.Linq;

namespace JK.Books.Implementation
{
    public class BookTypeCache : IEventHandler<EntityChangedEventData<BookType>>, IBookTypeCache
    {
        private readonly IBookTypeRepository _bookTypeRepository;
        private readonly ICacheManager _cacheManager;

        public BookTypeCache(IBookTypeRepository bookTypeRepository, ICacheManager cacheManager)
        {
            _bookTypeRepository = bookTypeRepository;
            _cacheManager = cacheManager;
        }
        public BookTypeDto Get(string name)
        {
            var bookType = GetOrNull(name);
            if (bookType == null)
            {
                throw new AbpException("There is no book type with given book type name: " + name);
            }

            return bookType;
        }

        public BookTypeDto Get(int id)
        {
            var bookType = GetOrNull(id);
            if (bookType == null)
            {
                throw new AbpException("There is no book type with given book type id: " + id);
            }

            return bookType;
        }

        public IReadOnlyList<BookTypeDto> GetAll()
        {
            return _cacheManager.GetCache(nameof(BookTypeDto))
                .Get("All",
                    () =>
                    {
                        return _bookTypeRepository.GetAllList().MapTo<List<BookTypeDto>>();
                    });
        }

        public BookTypeDto GetOrNull(string name)
        {
            return GetAll().FirstOrDefault(t => t.Name == name);
        }

        public BookTypeDto GetOrNull(int id)
        {
            return GetAll().FirstOrDefault(t => t.Id == id);
        }

        public void HandleEvent(EntityChangedEventData<BookType> eventData)
        {
            _cacheManager.GetCache(nameof(AuthorDto)).Remove("All");
        }
    }
}