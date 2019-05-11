namespace JK.Payments.Enumerates
{
    /// <summary>
    /// 回调状态
    /// </summary>
    public enum CallbackStatus
    {
        /// <summary>
        /// 未回调
        /// </summary>
        Pending = 10,
        /// <summary>
        /// 回调成功
        /// </summary>
        Succeed = 20,
        /// <summary>
        /// 回调失败
        /// </summary>
        Failed = 40
    }

}
