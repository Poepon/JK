using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using JK.Logs.Dto;
using Microsoft.EntityFrameworkCore;

namespace JK.Logs
{
   
    [AbpAuthorize]
    public class UserLoginAppService : JKAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;

        public UserLoginAppService(IRepository<UserLoginAttempt, long> userLoginAttemptRepository)
        {
            _userLoginAttemptRepository = userLoginAttemptRepository;
        }

        [DisableAuditing]
        public async Task<PagedResultDto<UserLoginAttemptDto>> GetUserLoginAttempts(GetUserLoginAttemptsInput input)
        {
            if (await IsGrantedAsync("Pages.LoginLogs"))
            {
                if (AbpSession.TenantId.HasValue)
                {
                    input.TenantId = AbpSession.TenantId;
                }
            }
            else
            {
                input.TenantId = AbpSession.TenantId;
                input.UserId = AbpSession.UserId;
            }
            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = _userLoginAttemptRepository.GetAll()
               .WhereIf(input.UserId.HasValue, la => la.UserId == input.UserId)
               .WhereIf(!string.IsNullOrEmpty(input.UserName), la => la.UserNameOrEmailAddress == input.UserName)
               .WhereIf(input.StartTime.HasValue, la => la.CreationTime >= input.StartTime)
               .WhereIf(input.EndTime.HasValue, la => la.CreationTime <= input.EndTime);

                int totalCount = query.Count();
                var loginAttempts = await query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<UserLoginAttemptDto>(totalCount, ObjectMapper.Map<List<UserLoginAttemptDto>>(loginAttempts));
            }
        }
    }
}
