using Abp.Authorization;
using JK.Authorization.Roles;
using JK.Authorization.Users;

namespace JK.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
