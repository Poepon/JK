using JK.Payments.Bacis;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.TenantConfigs
{
    [Table("TenantPaymentAppChannel")]
    public class TenantPaymentAppChannel
    {
        public int AppId { get; set; }

        [ForeignKey(nameof(AppId))]
        public virtual TenantPaymentApp App { get; set; }

        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }
    }
}
