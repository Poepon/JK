using JK.Payments.Enumerates;

namespace JK.Payments.ThirdParty
{
    /// <summary>
    /// 回调配置
    /// </summary>
    public class CallbackConfiguration
    {
        public int CompanyId { get; set; }

        public CallbackType CallbackType { get; set; }
    }
}
