namespace JK.Web.Public.Dto
{
    public class CloseOrderDto : PaymentApiDtoBase
    {
        public string ExternalOrderId { get; set; }

        public override string GetSignContent()
        {
            return $"{ExternalOrderId}{NonceStr}";
        }
    }
}
