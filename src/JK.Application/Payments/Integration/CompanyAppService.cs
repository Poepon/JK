using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using JK.Payments.Integration.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Extensions;

namespace JK.Payments.Integration
{
    public class CompanyAppService : JKAppServiceBase, ICompanyAppService
    {
        private readonly IRepository<Company> _companyRepository;

        public CompanyAppService(IRepository<Company> companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public async Task<CompanyDto> Create(CompanyEditDto input)
        {
            var company = ObjectMapper.Map<Company>(input);
            SetPaymentMethods(company, input.ChannelIds);
            await _companyRepository.InsertAsync(company);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<CompanyDto>(company);
        }

        [UseCase(Description = "删除支付公司")]
        public async Task Delete(EntityDto<int> input)
        {
            await _companyRepository.DeleteAsync(input.Id);
        }

        public async Task<CompanyDto> Get(EntityDto<int> input)
        {
            var entity = await _companyRepository.GetAsync(input.Id);
            return ObjectMapper.Map<CompanyDto>(entity);
        }

        public async Task<PagedResultDto<CompanyDto>> GetAll(PagedAndSortedResultRequestDto input)
        {
            var query = _companyRepository.GetAll();

            int totalCount = query.Count();
            if (input.Sorting.IsNullOrEmpty())
                input.Sorting = "Id";
            query = query.OrderBy(input.Sorting);
            query = query.PageBy(input);

            var entities = await query.ToListAsync();
            var list = ObjectMapper.Map<List<CompanyDto>>(entities);
            return new PagedResultDto<CompanyDto>(totalCount, list);
        }

        public async Task<CompanyEditDto> GetCompanyForEdit(EntityDto input)
        {
            var entity = await _companyRepository.GetAll().Include(c => c.SupportedChannels).SingleOrDefaultAsync(c => c.Id == input.Id);
            var dto = ObjectMapper.Map<CompanyEditDto>(entity);
            dto.ChannelIds = entity.SupportedChannels.Select(c => c.ChannelId).ToArray();
            return dto;
        }

        [UseCase(Description = "修改支付公司")]
        public async Task<CompanyDto> Update(CompanyEditDto input)
        {
            var company = await _companyRepository.GetAsync(input.Id);
            if (company == null)
            {
                throw new Exception("该支付公司不存在或已删除。");
            }
            ObjectMapper.Map(input, company);
            SetPaymentMethods(company, input.ChannelIds);
            await _companyRepository.UpdateAsync(company);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<CompanyDto>(company);
        }


        private void SetPaymentMethods(Company company, int[] channelIds)
        {
            if (company.Id > 0)
                _companyRepository.EnsureCollectionLoaded(company, u => u.SupportedChannels);
            else
                company.SupportedChannels = new List<CompanyChannel>();
            //Remove from removed methods
            foreach (var item in company.SupportedChannels.ToList())
            {
                if (channelIds.All(channelId => item.ChannelId != channelId))
                {
                    company.SupportedChannels.RemoveAll(tt => tt.ChannelId == item.ChannelId);
                }
            }

            //Add to added methods
            foreach (var channelId in channelIds)
            {
                // var method = _paymentMethodRepository.Get(methodId);
                if (company.SupportedChannels.All(tt => tt.ChannelId != channelId))
                {
                    company.SupportedChannels.Add(new CompanyChannel()
                    {
                        CompanyId = company.Id,
                        ChannelId = channelId
                    });
                }
            }
        }

    }
}
