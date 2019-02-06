namespace JK.Chat.Dto
{
    public enum MessageType : int
    {
        #region 用户
        /// <summary>
        /// 上线
        /// </summary>
        Online = 1,
        /// <summary>
        /// 下线
        /// </summary>
        Offline = 2,
        /// <summary>
        /// 屏蔽
        /// </summary>
        BlockUser = 3,
        /// <summary>
        /// 解除屏蔽
        /// </summary>
        UnblockUser = 4,
        #endregion

        #region 消息
        /// <summary>
        /// 输入中
        /// </summary>
        Typing = 10,
        /// <summary>
        /// 发送消息
        /// </summary>
        SendMessage = 11,
        /// <summary>
        /// 接收消息
        /// </summary>
        GetMessage = 12,
        /// <summary>
        /// 阅读消息
        /// </summary>
        ReadMessage = 13,
        /// <summary>
        /// 置顶消息
        /// </summary>
        PinMessageToTop = 14,
        /// <summary>
        /// 解除置顶消息
        /// </summary>
        UnpinMessageFromTop = 15,
        /// <summary>
        /// 上传文件
        /// </summary>
        UploadFile = 16,
        /// <summary>
        /// 下载文件
        /// </summary>
        DownloadFile = 17,
        #endregion

        #region 群组
        /// <summary>
        /// 创建群
        /// </summary>
        CreateGroup = 20,
        /// <summary>
        /// 删除群
        /// </summary>
        DeleteGroup = 21,
        /// <summary>
        /// 加入群
        /// </summary>
        JoinGroup = 22,
        /// <summary>
        /// 离开群
        /// </summary>
        LeaveGroup = 23,
        /// <summary>
        /// 置顶
        /// </summary>
        PinToTop = 24,
        /// <summary>
        /// 解除置顶
        /// </summary>
        UnpinFromTop = 25,
        #endregion

    }
}
