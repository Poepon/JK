using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Dependency;
using JK.Books.Dto;

namespace JK.Books.Interfaces
{
    public interface IBookVolumeAppService : IApplicationService, ITransientDependency
    {
        List<BookVolumeListDto> GetBookVolumes(int bookId);

    }
}
