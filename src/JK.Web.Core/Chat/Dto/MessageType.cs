namespace JK.Chat.Dto
{
    public enum MessageType : int
    {
        /// <summary>
        /// 上线
        /// </summary>
        Online = 1,
        /// <summary>
        /// 下线
        /// </summary>
        Offline = 2,
        /// <summary>
        /// 输入中
        /// </summary>
        Typing = 3,
        /// <summary>
        /// 发送消息
        /// </summary>
        SendMessage = 4,
        /// <summary>
        /// 接收消息
        /// </summary>
        GetMessage = 5,
        /// <summary>
        /// 阅读消息
        /// </summary>
        ReadMessage = 6,
        /// <summary>
        /// 加入群
        /// </summary>
        JoinGroup = 7,
        /// <summary>
        /// 离开群
        /// </summary>
        LeaveGroup = 8,
        /// <summary>
        /// 置顶
        /// </summary>
        PinToTop = 9,
        /// <summary>
        /// 解除置顶
        /// </summary>
        UnpinFromTop = 10,
        /// <summary>
        /// 屏蔽
        /// </summary>
        BlockUser = 11,
        /// <summary>
        /// 解除屏蔽
        /// </summary>
        UnblockUser = 12,
        /// <summary>
        /// 上传文件
        /// </summary>
        UploadFile = 13,
        /// <summary>
        /// 下载文件
        /// </summary>
        DownloadFile = 14
    }
}
