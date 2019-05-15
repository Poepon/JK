using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Payments.System.Dto
{
    public class SystemConfigurationDto : EntityDto
    {
        public string HttpCallbackUrl { get; set; }

        public string HttpsCallbackUrl { get; set; }
    }
}
