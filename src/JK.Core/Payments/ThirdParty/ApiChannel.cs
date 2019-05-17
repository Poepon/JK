using JK.Payments.Bacis;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.ThirdParty
{
    public class ApiChannel
    {
        public int ApiId { get; set; }

        [ForeignKey(nameof(ApiId))]
        public virtual ApiConfiguration Api { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}
