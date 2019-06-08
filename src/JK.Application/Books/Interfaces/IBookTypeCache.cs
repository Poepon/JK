using System.Collections.Generic;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookTypeCache : ITransientDependency
    {
        BookTypeDto Get(string name);
        
        BookTypeDto Get(int id);
        
        BookTypeDto GetOrNull(string name);
        
        BookTypeDto GetOrNull(int id);

        IReadOnlyList<BookTypeDto> GetAll();
    }
}