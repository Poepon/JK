using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Common
{
    public class GenericAttribute : Entity
    {
        public int? TenantId { get; set; }

        public string KeyGroup { get; set; }

        public int EntityId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
