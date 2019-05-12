using System;
using Abp.AutoMapper;

namespace JK.Payments.Orders.Dto
{
    [AutoMap(typeof(PaymentOrder))]
    public class PaymentOrderDto
    {
        public PaymentOrderDto()
        {
        }
    }
}
