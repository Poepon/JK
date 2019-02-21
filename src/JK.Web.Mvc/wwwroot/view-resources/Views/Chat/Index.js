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
            props: ['groups'],
            template: "#groupsContainerTemplate",
            methods: {

            }
        });
        Vue.component('vue-newgroup-form', {
            template: "#newGroupFormTemplate",
            data: function () {
                return {
                    gname: ""
                };
            },
            methods: {
                createGroup: function () {
                    var commanddto = { gname: this.gname };
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.CreateGroup, chat.dataType.MessagePack, bytes);
                    this.gname = "";
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
            props: ['currentgroup', 'allgroupsmessages', 'readystate', "loginuid"],
            template: "#messagesContainerTemplate",
            data: function () {
                return {
                    message: ""
                };
            },
            computed: {
                messages: function () {
                    var rs = [];
                    for (var i = 0; i < this.allgroupsmessages.length; i++) {
                        if (this.allgroupsmessages[i].gid === this.currentgroup.gid) {
                            rs.push(this.allgroupsmessages[i]);
                        }
                    }
                    return rs.sort(function (a, b) { return a.mid - b.mid; });
                }
            },
            mounted: function () {
                this.getMessages();
            },
            methods: {
                getMessages: function () {
                    var oldcommanddto = {
                        gid: this.currentgroup.gid,
                        mid: 0,
                        d: 2,
                        mc: 100,
                        loop: false
                    };
                    var newcommanddto = {
                        gid: this.currentgroup.gid,
                        mid: 0,
                        d: 1,
                        mc: 100,
                        loop: true
                    };

                    var oldbytes = serializeMsgPack(oldcommanddto);
                    chat.sendCommand(chat.commandType.GetMessage, chat.dataType.MessagePack, oldbytes);
                    var newbytes = serializeMsgPack(newcommanddto);
                    chat.sendCommand(chat.commandType.GetMessage, chat.dataType.MessagePack, newbytes);
                },
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
                    //this.$refs.messagespanel.scrollBy(0, this.$refs.messagespanel.scrollHeight);
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
                readystate: 3,
                friends: [

                ],
                groups: [

                ],
                allgroupsmessages: []
            },           
            methods: {
                getGroups: function () {
                    var commanddto = {};
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.GetGroups, chat.dataType.MessagePack, bytes);
                },
                getOnlineUsers: function () {
                    var commanddto = {};
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.GetOnlineUsers, chat.dataType.MessagePack, bytes);
                }
            }
        });

        abp.event.on("websocket.onopen", function (data) {
            chatApp.readystate = 1;
            chatApp.getGroups();
            //chatApp.getOnlineUsers();
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
                        chatApp.allgroupsmessages = chatApp.allgroupsmessages.concat(decodedObj);
                        break;
                    case chat.commandType.GetGroups:
                        console.log(decodedObj);
                        chatApp.groups = decodedObj;
                        break;
                    case chat.commandType.AlertMessage:
                        alert(decodedObj.text);
                        break;
                    case chat.commandType.GetOnlineUsers:
                        console.log(decodedObj);
                        chatApp.friends = decodedObj;
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
    });
})();