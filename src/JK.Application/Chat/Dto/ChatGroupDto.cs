using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Chat.Dto
{
    [AutoMap(typeof(ChatGroup))]
    public class ChatGroupDto : EntityDto<long>
    {
        public string Name { get; set; }

        public ChatGroupType GroupType { get; set; }

        public long? CreatorUserId { get; set; }

        public long CreationTime { get; set; }

        public int Status { get; set; }
    }
}
