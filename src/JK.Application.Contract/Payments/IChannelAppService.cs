using Abp.Application.Services;
using JK.Payments.Dto;

namespace JK.Payments
{
    public interface IChannelAppService : IAsyncCrudAppService<ChannelDto>
    {

    }
}