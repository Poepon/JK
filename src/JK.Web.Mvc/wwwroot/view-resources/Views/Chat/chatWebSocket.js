var chat = {};
chat.commandType = {
    /// <summary>
    /// 已连接
    /// </summary>
    Connected: 1,
    /// <summary>
    /// 断开连接
    /// </summary>
    Disconnected: 3,
    /// <summary>
    /// 错误
    /// </summary>
    AlertMessage: 4,
    /// <summary>
    /// 上线
    /// </summary>
    Online: 101,
    /// <summary>
    /// 下线
    /// </summary>
    Offline: 102,
    /// <summary>
    /// 屏蔽
    /// </summary>
    BlockUser: 103,
    /// <summary>
    /// 解除屏蔽
    /// </summary>
    UnblockUser: 104,

    /// <summary>
    /// 输入中
    /// </summary>
    Typing: 201,
    /// <summary>
    /// 发送消息
    /// </summary>
    SendMessage: 202,
    /// <summary>
    /// 接收消息
    /// </summary>
    GetMessage: 203,
    /// <summary>
    /// 阅读消息
    /// </summary>
    ReadMessage: 204,
    /// <summary>
    /// 置顶消息
    /// </summary>
    PinMessageToTop: 205,
    /// <summary>
    /// 解除置顶消息
    /// </summary>
    UnpinMessageFromTop: 206,
    /// <summary>
    /// 上传文件
    /// </summary>
    UploadFile: 207,
    /// <summary>
    /// 下载文件
    /// </summary>
    DownloadFile: 208,

    /// <summary>
    /// 创建私聊
    /// </summary>
    CreatePrivate: 300,
    /// <summary>
    /// 创建群组
    /// </summary>
    CreateGroup: 301,
    /// <summary>
    /// 删除群组
    /// </summary>
    DeleteGroup: 302,
    /// <summary>
    /// 加入群组
    /// </summary>
    JoinGroup: 303,
    /// <summary>
    /// 离开群组
    /// </summary>
    LeaveGroup: 304,
    /// <summary>
    /// 获取群组
    /// </summary>
    GetGroups: 305,
    /// <summary>
    /// 置顶
    /// </summary>
    PinToTop: 306,
    /// <summary>
    /// 解除置顶
    /// </summary>
    UnpinFromTop: 307,
    /// <summary>
    /// 获取在线用户
    /// </summary>
    GetOnlineUsers: 308
};
chat.dataType = {
    Text: 0,
    Json: 1,
    MessagePack: 2,
    Protobuf: 3,
    Blob: 4
};
(function () {
    chat.littleEndian = (function () {
        var buffer = new ArrayBuffer(2);
        new DataView(buffer).setInt16(0, 256, true);
        return new Int16Array(buffer)[0] === 256;
    })();

    chat.readyState = 3;
    chat.init = function () {
        var scheme = document.location.protocol === "https:" ? "wss" : "ws";
        var port = document.location.port ? (":" + document.location.port) : "";

        var connectionUrl = scheme + "://" + document.location.hostname + port + "/chatws";
        var ws = new ReconnectingWebSocket(connectionUrl, null, { binaryType: "arraybuffer" });
        ws.onconnecting = function (event) {
            chat.readyState = ws.readyState;
            console.log("onconnecting", ws.readyState);
            abp.event.trigger("websocket.onconnecting", event);
        };
        ws.onopen = function (event) {
            chat.readyState = ws.readyState;
            abp.event.trigger("websocket.onopen", event);
            console.log("onopen", ws.readyState);
        };
        ws.onmessage = function (event) {
            console.log("onmessage", event);
            if (event.data instanceof ArrayBuffer) {
                abp.event.trigger("websocket.onreceivebinary", event.data);
            }
            else if (event.data instanceof Blob) {
                abp.event.trigger("websocket.onreceiveblob", event.data);
            }
            else {
                abp.event.trigger("websocket.onreceivetext", event.data);
            }

        };
        ws.onclose = function (event) {
            chat.readyState = ws.readyState;
            abp.event.trigger("websocket.onclose", event);
            console.log("onclose", ws.readyState);
        };
        ws.onerror = function (event) {
            abp.event.trigger("websocket.onerror", event);
        };

        abp.event.on("websocket.sendbinary", function (data) {
            ws.send(data);
        });
        chat.ws = ws;
    };

    chat.sendCommand = function (commandType, dataType, dataBytes) {
        var buffer = new ArrayBuffer(9 + dataBytes.length);
        var dv = new DataView(buffer);
        dv.setInt32(0, commandType, chat.littleEndian);
        dv.setInt8(4, dataType);
        dv.setInt32(5, dataBytes.length, chat.littleEndian);
        for (var i = 0; i < dataBytes.length; i++) {
            dv.setUint8(i + 9, dataBytes[i]);
        }
        chat.ws.send(dv);
    };

    chat.receiveCommand = function (data) {
        var receive = [];
        var length = 0;
        receive = receive.concat(Array.from(new Uint8Array(data)));
        var dv = new DataView(data);
        var commandType = dv.getInt32(0, chat.littleEndian);
        var dataType = dv.getInt8(4, chat.littleEndian);
        length = dv.getInt32(5, chat.littleEndian);
        if (receive.length < length + 9) {
            return;
        }
        var bytes = receive.slice(9, length + 9);
        console.log(length);
        console.log(receive.length);
        return { commandType: commandType, dataType: dataType, data: bytes };
    };

    chat.online = function () {
        if (chat.ws) {
            chat.ws.open();
        }
    };
    chat.offline = function () {
        if (chat.ws) {
            chat.ws.close();
        }
    };
})();
