using Abp.AutoMapper;
using AutoMapper;
using JK.Payments.Dto;
using JK.Payments.Integration.Dto;
using JK.Payments.Integration.Dto.ApiParameters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using IObjectMapper = Abp.ObjectMapping.IObjectMapper;

namespace JK.Web.Areas.Payments.Models.ApiParameters
{
    [AutoMapFrom(typeof(ApiParameterDto))]
    public class EditViewModel : ApiParameterEditDto
    {
        [IgnoreMap]
        public IReadOnlyList<SelectListItem> Companies { get; set; }

        [IgnoreMap]
        public IReadOnlyList<SelectListItem> Channels { get; set; }

        public EditViewModel(ApiParameterEditDto input, IReadOnlyList<CompanyDto> companies, IReadOnlyList<ChannelDto> channels,IObjectMapper objectMapper)
        {
            objectMapper.Map(input, this);
            Companies = companies.Select(x => new SelectListItem(x.Name, x.Id.ToString(), this.CompanyId == x.Id, !x.IsActive)).ToList();
            Channels = channels.Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Exists(this.ChannelIds), !x.IsActive)).ToList();
        }
    }
}