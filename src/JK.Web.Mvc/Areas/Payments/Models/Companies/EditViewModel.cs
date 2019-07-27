using Abp.AutoMapper;
using AutoMapper;
using JK.Payments.Dto;
using JK.Payments.Integration.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using IObjectMapper = Abp.ObjectMapping.IObjectMapper;

namespace JK.Web.Areas.Payments.Models.Companies
{
    [AutoMapFrom(typeof(CompanyEditDto))]
    public class EditViewModel : CompanyEditDto
    {
        public EditViewModel(CompanyEditDto dto, IReadOnlyList<ChannelDto> channels,IObjectMapper objectMapper)
        {
            objectMapper.Map(dto, this);
            this.Channels = channels.Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Exists(this.ChannelIds), !c.IsActive)).ToList();
        }

        [IgnoreMap]
        public IReadOnlyList<SelectListItem> Channels { get; set; }
    }
}
