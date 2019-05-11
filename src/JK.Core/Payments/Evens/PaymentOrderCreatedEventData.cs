using Abp.Events.Bus;
using JK.Payments.Orders;

namespace JK.Payments.Evens
{
    public class PaymentOrderCreatedEventData : EventData
    {
        public PaymentOrder PaymentOrder { get; set; }

        public PaymentOrderCreatedEventData(PaymentOrder paymentOrder)
        {
            PaymentOrder = paymentOrder; ;
        }
    }

}
