using Abp.Application.Services;
using Abp.Domain.Repositories;
using JK.Payments.Bacis;
using JK.Payments.Dto;

namespace JK.Payments
{
    public class BankAppService :AsyncCrudAppService<Bank, BankDto>, IBankAppService
    {
        public BankAppService(IRepository<Bank, int> repository) : base(repository)
        {
        }
    }
}
