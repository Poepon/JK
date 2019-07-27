using Abp.Application.Services;
using JK.Payments.Dto;

namespace JK.Payments
{
    public interface IBankAppService : IAsyncCrudAppService<BankDto>
    {

    }
}
