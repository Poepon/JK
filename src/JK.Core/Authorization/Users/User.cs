using System;
using Abp.Authorization.Users;
using Abp.Extensions;

namespace JK.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public string GoogleAuthenticatorKey { get; set; }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string username)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName
            };

            user.SetNormalizedNames();

            return user;
        }
    }
}
