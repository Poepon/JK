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
};
chat.dataType = {
    Text: 0,
    Json: 1,
    MessagePack: 2,
    Protobuf: 3,
    Blob: 4
};
$(function () {
    var scheme = document.location.protocol === "https:" ? "wss" : "ws";
    var port = document.location.port ? (":" + document.location.port) : "";

    var connectionUrl = scheme + "://" + document.location.hostname + port + "/chatws";
    var ws = new ReconnectingWebSocket(connectionUrl, null, { binaryType: "arraybuffer" });
    ws.onconnecting = function () {
        console.log("onconnecting", ws.readyState);
    };
    ws.onopen = function () {
        $("#btnOpen").hide();
        $("#btnClose").show();
        console.log("onopen", ws.readyState);
    };
    ws.onmessage = function (event) {
        console.log("onmessage", event);
        if (event.data instanceof ArrayBuffer) {
            var receive = [];
            var length = 0;
            receive = receive.concat(Array.from(new Uint8Array(event.data)));
            if (receive.length < 9) {
                return;
            }
            var dv = new DataView(event.data);
            //使用小端字节序
            var commandType = dv.getInt32(0,true);
            var dataType = dv.getInt8(4, true);
            length = dv.getInt32(5, true);
            console.log("commandType", commandType);
            console.log("dataType", dataType);
            console.log("byteLength", receive.byteLength);
            console.log("length", length);
            if (receive.length < length + 9) {
                return;
            }
            var bytes = receive.slice(9, length + 9);
            if (dataType === chat.dataType.MessagePack) {
                var decodedData = deserializeMsgPack(bytes);
                console.log("decodedData", decodedData);
            }
          
            receive = receive.slice(length + 9);
            console.log("decodedData", receive);
        } else {
            var dto = JSON.parse(event.data);
            switch (dto.CommandType) {
                case chat.commandType.Connected:
                    var message = JSON.parse(dto.Data);
                    abp.utils.setCookieValue("chatConnectionId", message.ConnectionId);
                    break;

                default:
            }
        }


    };
    ws.onclose = function (event) {
        $("#btnClose").hide();
        $("#btnOpen").show();
        console.log();
        console.log("onclose", ws.readyState);
    };
    ws.onerror = function (event) {
        console.log("onerror", event);
    };

    $("#message").focus();
    $("#btnOpen").click(function () {
        ws.open();
    });
    $("#btnClose").click(function () {
        ws.close();
    });
    $("#btnSend").click(SendMessage);
    $("#message").keyup(function (event) {
        if (event.keyCode === 13) {
            SendMessage();
        }
    });
    function Send(commandType, dataType, Data) {
        var data = {
            CommandType: commandType,
            DataType: dataType,
            Data: JSON.stringify(Data)
        };
        ws.send(JSON.stringify(data));
    }
    function SendMessage() {
        var messagetext = $("#message").val();
        if (messagetext.trim() === "") {
            return;
        }
        var sendMessageDto = {
            GroupId: 1,
            UserId: 1,
            Message: messagetext
        };
        Send(chat.commandType.SendMessage, chat.dataType.Json, sendMessageDto);
        $("#message").val("");
    }
});