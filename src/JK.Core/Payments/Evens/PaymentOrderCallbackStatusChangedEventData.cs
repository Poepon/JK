using Abp.Events.Bus;
using JK.Payments.Enumerates;
using JK.Payments.Orders;

namespace JK.Payments.Evens
{
    public class PaymentOrderCallbackStatusChangedEventData : EventData
    {
        public PaymentOrder PaymentOrder { get; set; }

        public CallbackStatus OldStatus { get; set; }

        public string Reason { get; set; }

        public PaymentOrderCallbackStatusChangedEventData(PaymentOrder paymentOrder, CallbackStatus oldStatus)
        {
            PaymentOrder = paymentOrder;
            OldStatus = oldStatus;
        }
        public PaymentOrderCallbackStatusChangedEventData(PaymentOrder paymentOrder, CallbackStatus oldStatus, string reason)
        {
            PaymentOrder = paymentOrder;
            OldStatus = oldStatus;
            Reason = reason;
        }
    }

}
