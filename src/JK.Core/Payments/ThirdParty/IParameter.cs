using JK.Payments.Enumerates;

namespace JK.Payments.ThirdParty
{
    public interface IParameter
    {
        string Key { get; set; }

        ExpressionType ExpType { get; set; }

        string Expression { get; set; }

        DataTag? DataTag { get; set; }
    }
}
