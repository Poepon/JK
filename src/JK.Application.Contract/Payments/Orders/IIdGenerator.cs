using Abp.Dependency;

namespace JK.Payments.Orders
{
    public interface IIdGenerator : ISingletonDependency
    {
        long NextId();

        void Init(uint dataCenterId, uint workerId);
    }
}
