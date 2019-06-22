using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Bacis;

namespace JK.Payments.Integration
{
    [Table("ParameterChannel")]
    public class ParameterChannel
    {
        public int ParameterId { get; set; }

        [ForeignKey(nameof(ParameterId))]
        public virtual ApiParameter Parameter { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}