using JK.Payments.TenantConfigs;
using System.Linq;

namespace JK.Payments.Orders
{
    public enum BuiltInParameterType
    {
        Global = 0,
        Config = 1,
        Order = 2,
        Parameter = 3
    }
    public class BuiltInParameters
    {
        public string Key { get; set; }

        public BuiltInParameterType ParameterType { get; set; }

        public string DataTypeName { get; set; }

        public string Desc { get; set; }
    }

}
