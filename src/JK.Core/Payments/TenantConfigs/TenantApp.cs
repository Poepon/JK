using Abp.Domain.Entities;
using JK.Payments.Bacis;
using JK.Payments.Enumerates;
using JK.Payments.ThirdParty;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    public class TenantApp : Entity
    {
        public int TenantId { get; set; }

        public string Name { get; set; }

        public string AppId { get; set; }

        public string Key { get; set; }

        public DeviceType Device { get; set; }

        public virtual ICollection<TenantAppChannel> SupportedChannels { get; set; }

        public virtual ICollection<TenantAppCompany> SupportedCompanies { get; set; }
    }
    public class TenantAppCompany
    {
        public int TenantAppId { get; set; }

        [ForeignKey(nameof(TenantAppId))]
        public virtual TenantApp App { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
    }
    
    public class TenantAppChannel
    {
        public int TenantAppId { get; set; }

        [ForeignKey(nameof(TenantAppId))]
        public virtual TenantApp App { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}
