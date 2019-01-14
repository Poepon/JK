using System.Collections.Generic;
using JK.Roles.Dto;
using JK.Users.Dto;

namespace JK.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
