namespace JK.Payments.Orders.Dto
{
    public class VerifiedPaymentOrderDto
    {
        public CreatePaymentOrderDto PaymentOrder { get; set; }

        public bool Success { get; set; }

        public int ChannelId { get; set; }

        public int? BankId { get; set; }

        public int CompanyId { get; set; }

        public int AccountId { get; set; }
    }
}
