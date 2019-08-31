using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JK.Payments.Dto;
using JK.Payments.Integration.Dto;
using JK.Payments.TenantConfigs.Dto;

namespace JK.Web.Areas.Payments.Models.PaymentApps
{
    [AutoMap(typeof(PaymentAppDto))]
    public class EditViewModel:PaymentAppDto
    {
        public IReadOnlyList<ChannelDto> Channels { get; set; }

        public IReadOnlyList<CompanyDto> Companies { get; set; }
    }
}
