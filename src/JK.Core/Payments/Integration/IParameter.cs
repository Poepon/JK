using JK.Payments.Enumerates;

namespace JK.Payments.Integration
{
    public interface ISetValueParameter
    {
        string Key { get; set; }

        string ValueOrExpression { get; set; }

        SetValueLocation Location { get; set; }
    }
    public interface IGetValueParameter
    {
        string Key { get; set; }

        string Expression { get; set; }

        DataTag? DataTag { get; set; }

        GetValueLocation Location { get; set; }
    }
}
