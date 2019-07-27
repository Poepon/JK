using System.ComponentModel.DataAnnotations.Schema;
using JK.Payments.Bacis;

namespace JK.Payments.Tenants
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
