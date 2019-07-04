using JK.Payments.Bacis;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    [Table("PaymentAppChannel")]
    public class PaymentAppChannel
    {
        public int AppId { get; set; }

        [ForeignKey(nameof(AppId))]
        public virtual PaymentApp App { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}
