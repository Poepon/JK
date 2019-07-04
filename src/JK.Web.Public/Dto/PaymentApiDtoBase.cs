namespace JK.Web.Public.Dto
{
    public abstract class PaymentApiDtoBase
    {
        public string AppId { get; set; }

        public string Sign { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string NonceStr { get; set; }

        public abstract string GetSignContent();
    }
}
