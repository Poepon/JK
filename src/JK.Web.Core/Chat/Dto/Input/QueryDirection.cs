namespace JK.Chat.Dto.Input
{
    public enum QueryDirection : byte
    {
        /// <summary>
        /// 向前查询,MessageId &lt; @MessageId
        /// </summary>
        Forward = 0,
        /// <summary>
        /// 向后查询,MessageId > @MessageId
        /// </summary>
        Backward = 1
    }
}
