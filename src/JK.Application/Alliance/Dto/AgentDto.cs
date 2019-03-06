using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Alliance.Dto
{
    [AutoMap(typeof(Agent))]
    public class AgentDto : EntityDto<long>
    {

    }
}
