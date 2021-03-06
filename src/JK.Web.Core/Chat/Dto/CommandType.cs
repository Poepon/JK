﻿namespace JK.Chat.Dto
{
    public enum CommandType : int
    {
        /// <summary>
        /// 已连接
        /// </summary>
        Connected = 1,
        /// <summary>
        /// 断开连接
        /// </summary>
        Disconnected = 3,
        /// <summary>
        /// 提示消息
        /// </summary>
        AlertMessage = 4,
        #region 用户
        /// <summary>
        /// 上线
        /// </summary>
        Online = 101,
        /// <summary>
        /// 下线
        /// </summary>
        Offline = 102,
        /// <summary>
        /// 屏蔽
        /// </summary>
        BlockUser = 103,
        /// <summary>
        /// 解除屏蔽
        /// </summary>
        UnblockUser = 104,
        #endregion

        #region 消息
        /// <summary>
        /// 输入中
        /// </summary>
        Typing = 201,
        /// <summary>
        /// 发送消息
        /// </summary>
        SendMessage = 202,
        /// <summary>
        /// 接收消息
        /// </summary>
        GetMessage = 203,
        /// <summary>
        /// 阅读消息
        /// </summary>
        ReadMessage = 204,
        /// <summary>
        /// 置顶消息
        /// </summary>
        PinMessageToTop = 205,
        /// <summary>
        /// 解除置顶消息
        /// </summary>
        UnpinMessageFromTop = 206,
        /// <summary>
        /// 上传文件
        /// </summary>
        UploadFile = 207,
        /// <summary>
        /// 下载文件
        /// </summary>
        DownloadFile = 208,
        #endregion

        #region 会话
        /// <summary>
        /// 创建私人会话
        /// </summary>
        CreatePrivateSession = 300,
        /// <summary>
        /// 创建公共会话
        /// </summary>
        CreatePublicSession = 301,
        /// <summary>
        /// 删除会话
        /// </summary>
        DeleteSession = 302,
        /// <summary>
        /// 加入会话
        /// </summary>
        JoinSession = 303,
        /// <summary>
        /// 离开会话
        /// </summary>
        LeaveSession = 304,
        /// <summary>
        /// 获取群组
        /// </summary>
        GetSessions = 305,
        /// <summary>
        /// 置顶
        /// </summary>
        PinToTop = 306,
        /// <summary>
        /// 解除置顶
        /// </summary>
        UnpinFromTop = 307,
        /// <summary>
        /// 获取在线用户
        /// </summary>
        GetOnlineUsers = 308,
        /// <summary>
        /// 获取未读消息数
        /// </summary>
        GetSessionUnread = 309,
        /// <summary>
        /// 获取最后一条消息
        /// </summary>
        GetSessionLastMessage = 310
        #endregion

    }
}
