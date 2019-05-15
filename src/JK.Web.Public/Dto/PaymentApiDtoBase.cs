namespace JK.Web.Public.Dto
{
    public abstract class PaymentApiDtoBase
    {
        public string AppId { get; set; }

        public string Sign { get; set; }

        public string NonceStr { get; set; }

        public abstract string GetSignContent();
    }
}
