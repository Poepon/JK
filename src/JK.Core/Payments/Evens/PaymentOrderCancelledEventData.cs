using Abp.Events.Bus;
using JK.Payments.Orders;

namespace JK.Payments.Evens
{
    public class PaymentOrderCancelledEventData : EventData
    {
        public PaymentOrder PaymentOrder { get; set; }
        public PaymentOrderCancelledEventData(PaymentOrder paymentOrder)
        {
            PaymentOrder = paymentOrder; ;
        }
    }

}
