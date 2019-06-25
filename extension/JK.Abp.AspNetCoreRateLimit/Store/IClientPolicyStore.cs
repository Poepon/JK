using Abp.Dependency;
using System.Threading.Tasks;

namespace JK.Abp.AspNetCoreRateLimit.Store
{
    public interface IClientPolicyStore: ISingletonDependency
    {
        Task SeedAsync();
    }
}