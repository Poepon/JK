using JK.Payments.Bacis;
using System.ComponentModel.DataAnnotations.Schema;

namespace JK.Payments.Orders
{
    [Table("PaymentOrderPolicyChannel")]
    public class PaymentOrderPolicyChannel
    {
        public int ChannelId { get; set; }

        [ForeignKey(nameof(ChannelId))]
        public virtual Channel Channel { get; set; }

        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public virtual PaymentOrderPolicy PaymentOrderPolicy { get; set; }
    }

}
