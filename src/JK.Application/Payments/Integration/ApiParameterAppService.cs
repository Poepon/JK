using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using JK.Payments.Integration.Dto.ApiParameters;

namespace JK.Payments.Integration
{
    public class ApiParameterAppService : CrudAppService<ApiParameter, ApiParameterDto, int, GetParameterListInput, ApiParameterEditDto>, IApiParameterAppService
    {
        public ApiParameterAppService(IRepository<ApiParameter, int> repository) : base(repository)
        {
        }

        public override ApiParameterDto Create(ApiParameterEditDto input)
        {
            return base.Create(input);
        }

        public ApiParameterEditDto GetParameterForEdit(EntityDto input)
        {
            var entity = Repository.GetAllIncluding(p => p.SupportedChannels).SingleOrDefault(p => p.Id == input.Id);
            var dto = ObjectMapper.Map<ApiParameterEditDto>(entity);
            dto.ChannelIds = entity.SupportedChannels.Select(c => c.ChannelId).ToArray();
            return dto;
        }

        public override ApiParameterDto Update(ApiParameterEditDto input)
        {
            return base.Update(input);
        }
    }
}
