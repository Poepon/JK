using JK.Cryptography;

namespace JK.Payments.Orders
{
    public static class PaymentOrderExtensions
    {
        public static string GetMd5(this PaymentOrder paymentOrder)
        {
            string value = $"{paymentOrder.TenantId}.{paymentOrder.CompanyId}.{paymentOrder.ChannelId}.{paymentOrder.AccountId}.{paymentOrder.ExternalOrderId}.{paymentOrder.Amount}";
            return SecurityHelper.MD5(value, "");
        }

        public static bool VerifyMd5(this PaymentOrder paymentOrder)
        {
            string value = $"{paymentOrder.TenantId}.{paymentOrder.CompanyId}.{paymentOrder.ChannelId}.{paymentOrder.AccountId}.{paymentOrder.ExternalOrderId}.{paymentOrder.Amount}";
            return SecurityHelper.MD5(value, "") == paymentOrder.Md5;
        }
    }
}
