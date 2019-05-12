using System;
using JK.Dto;
using JK.Payments.Orders.Dto;

namespace JK.Payments.Orders
{
    public interface IOrderProcessingService
    {

        /// <summary>
        /// 提交一个支付请求
        /// </summary>
        ResultDto<PaymentOrderDto> PlaceOrder(CreatePaymentOrderDto input);
    }

    public class OrderProcessingService : IOrderProcessingService
    {
        public ResultDto<PaymentOrderDto> PlaceOrder(CreatePaymentOrderDto input)
        {
            throw new NotImplementedException();
        }
    }
}
