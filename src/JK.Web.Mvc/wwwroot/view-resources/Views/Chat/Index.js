(function () {
    $(function () {
        Vue.filter('grouptime', function (value) {
            if (!value) return '';
            return moment(value).format("ddd,HH:mm");
        });
        Vue.filter('timeago', function (value) {
            if (!value) return '';
            return moment(value).fromNow();
        });
        Vue.component('vue-groups-container', {
            props: ['groups', "currentgroup"],
            template: "#groupsContainerTemplate",
            methods: {
                changeGroup: function (newgroup) {
                    if (this.currentgroup.gid !== newgroup.gid) {
                        for (var i = 0; i < this.groups.length; i++) {
                            if (this.groups[i].cur)
                                this.groups[i].cur = false;
                        }
                        newgroup.cur = true;
                        this.$root.currentgroup = newgroup;
                    }
                }
            }
        });
        Vue.component('vue-newgroup-form', {
            template: "#newGroupFormTemplate",
            data: function () {
                return {
                    gn:""
                };
            },
            methods: {
                createGroup: function () {
                    var commanddto = { gn: this.gn };
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.CreateGroup, chat.dataType.MessagePack, bytes);
                    this.gn = "";
                }
            }
        });
        Vue.component('vue-friends-container', {
            props: ['friends'],
            template: "#friendsContainerTemplate",
            methods: {
                goChat: function (uid) {
                    var commanddto = { tguid: uid };
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.CreatePrivate, chat.dataType.MessagePack, bytes);
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
                    if (this.readystate !== 1) {
                        alert("未能连接到服务器，请检查网络后刷新重试。");
                    }
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
                currentgroup: { gn: "", gid: "" },
                readystate: 3,
                friends: [
                    { uid: 1, uname: "张一疯", ico: "/images/user.png" },
                    { uid: 2, uname: "张二疯", ico: "/images/user.png" },
                    { uid: 3, uname: "张三疯", ico: "/images/user.png" }
                ],
                messages: [

                ],
                groups: [
                    
                ]
            },
            methods: {
                getGroups: function () {
                    var commanddto = {};
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.GetGroups, chat.dataType.MessagePack, bytes);
                }
            },
           
            watch: {
                currentgroup: function (val, oldval) {
                    this.messages = [];
                    var oldcommanddto = {
                        gid: val.gid,
                        mid: 0,
                        d: 2,
                        mc: 100,
                        loop: false
                    };
                    var newcommanddto = {
                        gid: val.gid,
                        mid: 0,
                        d: 1,
                        mc: 100,
                        loop:true
                    };
                  
                    var oldbytes = serializeMsgPack(oldcommanddto);
                    chat.sendCommand(chat.commandType.GetMessage, chat.dataType.MessagePack, oldbytes);
                    var newbytes = serializeMsgPack(newcommanddto);
                    chat.sendCommand(chat.commandType.GetMessage, chat.dataType.MessagePack, newbytes);
                }
            }
        });

        abp.event.on("websocket.onopen", function (data) {
            chatApp.readystate = 1;
            chatApp.getGroups();
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
                switch (cmddto.commandType) {
                    case chat.commandType.GetMessage:
                        chatApp.messages = chatApp.messages.concat(decodedObj).sort(function (a, b) { return a.mid - b.mid; });
                        break;
                    case chat.commandType.GetGroups:
                        chatApp.groups = decodedObj;
                        break;
                    case chat.commandType.AlertMessage:
                        alert(decodedObj.text);
                        break;
                    default:
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