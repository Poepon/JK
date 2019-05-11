using Abp.Events.Bus;
using JK.Payments.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Payments.Evens
{
    public class PaymentOrderPaidEventData : EventData
    {
        public PaymentOrder PaymentOrder { get; set; }

        public PaymentOrderPaidEventData(PaymentOrder paymentOrder)
        {
            PaymentOrder = paymentOrder; ;
        }
    }

}
