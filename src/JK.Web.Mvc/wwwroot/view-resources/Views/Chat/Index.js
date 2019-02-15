(function () {
    $(function () {
        Vue.component('vue-groups-container', {
            props: ['groups', "currentgroup"],
            template: "#groupsContainerTemplate",
            methods: {
                changeGroup: function (newgroup) {
                    if (this.currentgroup.gid !== newgroup.gid) {
                        for (var i = 0; i < this.groups.length; i++) {
                            if (this.groups[i].isCurrent)
                                this.groups[i].isCurrent = false;
                        }
                        newgroup.isCurrent = true;
                        this.$root.currentgroup = newgroup;
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
            props: ['currentgroup', 'readystate', 'messages',"loginuid"],
            template: "#messagesContainerTemplate",
            data: function () {
                return { message: "" };
            },
            methods: {
                sendMessage: function () {
                    if (this.message.trim() === "") {
                        return;
                    }
                    var messagedto = {
                        gid: this.currentgroup.gid,
                        uid: 1,
                        msg: this.message
                    };
                    var bytes = serializeMsgPack(messagedto);
                    chat.sendCommand(chat.commandType.SendMessage, chat.dataType.MessagePack, bytes);
                    this.message = "";
                    this.$refs.messagespanel.scrollBy(0, this.$refs.messagespanel.scrollHeight);
                },
                online: function () {
                    chat.online();
                },
                offline: function () {
                    chat.offline();
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
                        isCurrent: false
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

            },
            watch: {
                currentgroup: function (val, oldval) {
                    this.messages = [];
                    var commanddto = {
                        gid: val.gid,
                        mid:0
                    };
                  
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.GetMessage, chat.dataType.MessagePack, bytes);
                }
            }
        });

        abp.event.on("websocket.onopen", function (data) {
            chatApp.readystate = 1;
        });
        abp.event.on("websocket.onconnecting", function (data) {
            chatApp.readystate = 0;
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