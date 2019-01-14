using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using JK.Configuration;
using JK.Web;

namespace JK.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class JKDbContextFactory : IDesignTimeDbContextFactory<JKDbContext>
    {
        public JKDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<JKDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            JKDbContextConfigurer.Configure(builder, configuration.GetConnectionString(JKConsts.ConnectionStringName));

            return new JKDbContext(builder.Options);
        }
    }
}
