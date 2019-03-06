using Abp.Application.Services;
using Abp.Application.Services.Dto;
using JK.Alliance.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JK.Alliance
{
    public interface IAgentAppService : IApplicationService
    {
        Task<AgentDto> Create(AgentDto agent);

        Task<AgentDto> Update(AgentDto agent);

        Task Delete(NullableIdDto<long> entity);

        Task<AgentDto> Get(NullableIdDto<long> entity);

        Task<PagedResultDto<AgentDto>> GetPagedList(PagedAndSortedResultRequestDto input);
    }
}
