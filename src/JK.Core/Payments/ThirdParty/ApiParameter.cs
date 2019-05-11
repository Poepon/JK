using JK.Payments.Enumerates;
namespace JK.Payments.ThirdParty
{
    public class ApiParameter
    {
        public int ApiId { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; }


        public string Value { get; set; }

        public EncryptionMethod? Encryption { get; set; }

        public string Format { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }
    }
}
