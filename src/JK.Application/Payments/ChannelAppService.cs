using Abp.Application.Services;
using Abp.Domain.Repositories;
using JK.Payments.Bacis;
using JK.Payments.Dto;

namespace JK.Payments
{
    public class ChannelAppService : AsyncCrudAppService<Channel, ChannelDto>, IChannelAppService
    {
        public ChannelAppService(IRepository<Channel, int> repository) : base(repository)
        {
        }
    }
}