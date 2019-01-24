using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using JK.Authorization.Roles;
using JK.Authorization.Users;
using JK.MultiTenancy;
using JK.Storage;

namespace JK.EntityFrameworkCore
{
    public class JKDbContext : AbpZeroDbContext<Tenant, Role, User, JKDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public JKDbContext(DbContextOptions<JKDbContext> options)
            : base(options)
        {
        }
    }
}
