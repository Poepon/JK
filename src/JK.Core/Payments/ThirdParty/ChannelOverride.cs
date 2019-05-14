using Abp.Domain.Entities;
using JK.Payments.Bacis;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public class ChannelOverride : Entity
    {
        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        [Required]
        [StringLength(20)]
        public string OverrideCode { get; set; }

        public decimal? OverrideFeeRate { get; set; }
    }
}
