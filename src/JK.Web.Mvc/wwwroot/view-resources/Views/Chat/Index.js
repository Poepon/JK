(function () {
    $(function () {
        Vue.component('vue-groups-container', {
            props: ['groups', "currentgroup"],
            template: "#groupsContainerTemplate",
            methods: {
                changeGroup: function (oldgroup, newgroup) {
                    if (oldgroup !== newgroup) {
                        if (oldgroup) {
                            oldgroup.isCurrent = false;
                        }
                        newgroup.isCurrent = true;
                        currentgroup = newgroup;
                        console.log("changeGroup todo", currentgroup);
                    }
                }
            }
        });
        Vue.component('vue-friends-container', {
            props: ['friends'],
            template: "#friendsContainerTemplate",
            methods: {
                goChat: function (uid) {
                    console.log("goChat todo");
                }
            }
        });
        Vue.component('vue-messages-container', {
            props: ['currentgroup', 'readystate', 'messages'],
            template: "#messagesContainerTemplate",
            methods: {
                sendMessage: function () {
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
            }
        });
        var chatApp = new Vue({
            el: '#chatApp',
            data: {
                currentgroup: { gname: "", gid: "" },
                readystate: 3,
                friends: [
                    { uid: 1, uname: "张一疯", ico: "/images/user.png" },
                    { uid: 2, uname: "张二疯", ico: "/images/user.png" },
                    { uid: 3, uname: "张三疯", ico: "/images/user.png" }
                ],
                messages: [

                ],
                groups: [
                    {
                        gid: 1,
                        gname: "学前班1班",
                        ico: "/images/user.png",
                        lstmsg: "hello,nice to meet you.",
                        lsttime: "2019-02-14 14:23",
                        unread: 99,
                        isCurrent: true
                    },
                    {
                        gid: 2,
                        gname: "学前班2班",
                        ico: "/images/user.png",
                        lstmsg: "hello,nice to meet you.",
                        lsttime: "2019-02-14 14:23",
                        unread: 99,
                        isCurrent: false
                    }
                ]
            },
            methods: {

            }
        });

        abp.event.on("websocket.onopen", function (data) {
            chatApp.readystate = 1;
        });
        abp.event.on("websocket.onclose", function (data) {
            chatApp.readystate = 3;
        });
        abp.event.on("websocket.onerror", function (data) {
            console.log("onerror", event);
        });
        abp.event.on("websocket.onreceivebinary", function (data) {
            var cmddto = chat.receiveCommand(data);
            if (cmddto.dataType === chat.dataType.MessagePack) {
                var decodedObj = deserializeMsgPack(cmddto.data);
                if (cmddto.commandType === chat.commandType.GetMessage) {
                    chatApp.messages = chatApp.messages.concat(decodedObj);
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
    });
})();