using System.Threading.Tasks;
using Abp.Dependency;

namespace JK.Abp.AspNetCoreRateLimit.Store
{
    public interface IIpPolicyStore : ISingletonDependency
    {
        Task SeedAsync();
    }
}