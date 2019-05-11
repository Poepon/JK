using System;
using System.Collections.Generic;
using System.Text;

namespace JK.Payments.Enumerates
{
    /// <summary>
    /// 支付订单状态
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// 等待付款
        /// </summary>
        Pending = 10,
        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 20,
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 40,
    }
}
