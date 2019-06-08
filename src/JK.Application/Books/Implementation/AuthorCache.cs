using System.Linq;
using Abp;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using JK.Books.Dto;
using JK.Books.Interfaces;
using JK.IRepositories;

namespace JK.Books.Implementation
{
    public class AuthorCache : IEventHandler<EntityChangedEventData<Author>>, IAuthorCache
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ICacheManager _cacheManager;

        public AuthorCache(IAuthorRepository authorRepository, ICacheManager cacheManager)
        {
            _authorRepository = authorRepository;
            _cacheManager = cacheManager;
        }
        public AuthorDto Get(int id)
        {
            var author = GetOrNull(id);
            if (author == null)
            {
                throw new AbpException("There is no author with given author id: " + id);
            }
            return author;
        }

        public AuthorDto Get(string name)
        {
            var author = GetOrNull(name);
            if (author == null)
            {
                throw new AbpException("There is no author with given author name: " + name);
            }

            return author;
        }


        public AuthorDto GetOrNull(string name)
        {
            return _cacheManager.GetCache(nameof(AuthorDto))
                .Get(name,
                    () =>
                    {
                        return _authorRepository.FirstOrDefault(a => a.Name == name).MapTo<AuthorDto>();
                    });
        }

        public AuthorDto GetOrNull(int id)
        {
            return _cacheManager.GetCache(nameof(AuthorDto))
                .Get(id,
                    () =>
                    {
                        return _authorRepository.FirstOrDefault(id).MapTo<AuthorDto>();
                    });
        }

        public void HandleEvent(EntityChangedEventData<Author> eventData)
        {
            _cacheManager.GetCache(nameof(AuthorDto)).Remove(eventData.Entity.Id.ToString());
            _cacheManager.GetCache(nameof(AuthorDto)).Remove(eventData.Entity.Name.ToString());
        }
    }
}