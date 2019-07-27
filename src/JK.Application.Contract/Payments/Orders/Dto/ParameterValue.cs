using JK.Payments.Enumerates;

namespace JK.Payments.Orders.Dto
{
    public class ParameterValue
    {
        public ParameterValue()
        {

        }
        public ParameterValue(string key, string value, Location? location)
        {
            Key = key;
            Value = value;
            Location = location;
        }
        public string Key { get; set; }

        public string Value { get; set; }

        public Location? Location { get; set; }
    }
}