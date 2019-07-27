using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    public interface IValueParameter
    {
        string Key { get; set; }

        string ValueOrExpression { get; set; }

        DataTag? DataTag { get; set; }

        Location? Location { get; set; }
    }
}
