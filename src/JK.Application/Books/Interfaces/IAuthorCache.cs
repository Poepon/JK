using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IAuthorCache : ITransientDependency
    {
        AuthorDto Get(string name);

        AuthorDto Get(int id);

        AuthorDto GetOrNull(string name);

        AuthorDto GetOrNull(int id);
    }
}