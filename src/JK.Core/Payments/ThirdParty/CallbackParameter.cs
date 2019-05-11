using JK.Payments.Enumerates;

namespace JK.Payments.ThirdParty
{
    public class CallbackParameter
    {
        public int CallbackId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DataTag DataTag { get; set; }

        public string Desc { get; set; }

        public int OrderNumber { get; set; }

    }
}
