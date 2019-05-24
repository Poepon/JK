using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Bacis;

namespace JK.Payments.Integration
{
    [Table("CompanyChannel")]
    public class CompanyChannel
    {
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}
