using System;
using System.Linq;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.MultiTenancy;

namespace JK.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(Tenant),
            typeof(Role),
            typeof(User)
           
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(Role),
            typeof(User)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type=>type.FullName)
                .Select(types=>types.First())
                .ToArray();
    }
}
