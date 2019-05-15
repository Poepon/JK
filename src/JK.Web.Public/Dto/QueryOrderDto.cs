namespace JK.Web.Public.Dto
{
    public class QueryOrderDto : PaymentApiDtoBase
    {
        public string TransactionId { get; set; }

        public string ExternalOrderId { get; set; }

        public override string GetSignContent()
        {
            return $"{TransactionId}{ExternalOrderId}{NonceStr}";
        }
    }
}
