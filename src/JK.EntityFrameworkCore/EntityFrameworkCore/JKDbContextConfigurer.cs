using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace JK.EntityFrameworkCore
{
    public static class JKDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<JKDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<JKDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
