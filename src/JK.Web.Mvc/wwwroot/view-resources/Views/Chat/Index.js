(function(){
    $(function(){
         abp.event.on("websocket.onopen",function(data){
            $("#btnOpen").hide();
            $("#btnClose").show();
        });
        abp.event.on("websocket.onclose",function(data){
            $("#btnClose").hide();
            $("#btnOpen").show();
        });
        abp.event.on("websocket.onerror",function(data){
           console.log("onerror", event);
        });
        abp.event.on("websocket.onreceivebinary", function (data) {
            var cmddto = chat.receiveCommand(data);
            if (cmddto.dataType === chat.dataType.MessagePack) {
                var decodedObj = deserializeMsgPack(cmddto.data);
                if (cmddto.commandType === chat.commandType.GetMessage) {
                    console.log(decodedObj);
                }
            }
        });
        abp.event.on("websocket.onreceiveblob", function (data) {

        });
        abp.event.on("websocket.onreceivetext", function (data) {
            console.log(data);
            var dto = JSON.parse(data);
            switch (dto.CommandType) {
                case chat.commandType.Connected:
                    var message = JSON.parse(dto.Data);
                    abp.utils.setCookieValue("chatConnectionId", message.ConnectionId);
                    break;

                default:
            }
        });
        chat.init();
        $("#message").focus();

        $("#btnOpen").click(function () {
            chat.online();
        });
        $("#btnClose").click(function () {
            chat.offline();
        });
        $("#btnSend").click(SendMessage);
        $("#message").keyup(function (event) {
            if (event.keyCode === 13) {
                SendMessage();
            }
        });
        function SendMessage() {
            var messagetext = $("#message").val();
            if (messagetext.trim() === "") {
                return;
            }
            var messagedto = {
                gid: 1,
                uid: 1,
                msg: messagetext
            };
            var bytes = serializeMsgPack(messagedto);
            chat.sendCommand(chat.commandType.SendMessage, chat.dataType.MessagePack, bytes);
            $("#message").val("");
        }
    });
})();