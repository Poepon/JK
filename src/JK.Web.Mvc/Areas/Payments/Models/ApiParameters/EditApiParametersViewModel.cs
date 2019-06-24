using Abp.AutoMapper;
using AutoMapper;
using JK.Payments;
using JK.Payments.Integration.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using JK.Payments.Integration.Dto.ApiParameters;

namespace JK.Web.Areas.Payments.Models.ApiParameters
{
    [AutoMapFrom(typeof(ApiParameterDto))]
    public class EditApiParametersViewModel : ApiParameterEditDto
    {
        [IgnoreMap]
        public IReadOnlyList<SelectListItem> Companies { get; set; }

        [IgnoreMap]
        public IReadOnlyList<SelectListItem> Channels { get; set; }

        public bool IsEditMode
        {
            get { return Id > 0; }
        }

        public EditApiParametersViewModel(ApiParameterEditDto input, IReadOnlyList<CompanyDto> companies, IReadOnlyList<ChannelDto> channels)
        {
            input.MapTo(this);
            Companies = companies.Select(x => new SelectListItem(x.Name, x.Id.ToString(), this.CompanyId == x.Id)).ToList();
            Channels = channels.Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Exists(this.ChannelIds))).ToList();
        }
    }
}