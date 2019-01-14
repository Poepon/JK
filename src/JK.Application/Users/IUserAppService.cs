using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Roles.Dto;
using JK.Users.Dto;

namespace JK.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
