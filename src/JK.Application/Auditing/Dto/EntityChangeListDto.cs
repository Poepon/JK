using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.EntityHistory;

namespace JK.Auditing.Dto
{
    [AutoMapFrom(typeof(EntityChange))]
    public class EntityChangeListDto : EntityDto<long>
    {
        public long? UserId { get; set; }

        public string UserName { get; set; }

        public string Reason { get; set; }

        public DateTime ChangeTime { get; set; }

        public string EntityTypeFullName { get; set; }

        public string ChangeType { get; set; }

        public string ChangeTypeName => ChangeType.ToString();

        public long EntityChangeSetId { get; set; }
    }
}