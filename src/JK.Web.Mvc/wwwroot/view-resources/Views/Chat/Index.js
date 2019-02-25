(function () {
    $(function () {
        const store = new Vuex.Store({
            state: {
                readystate:3,
                groups: [],
                onlineusers: [],
                allmessages: []
            },
            mutations: {
                changereadystate(state, payload) {
                    state.readystate = payload.readystate;
                    console.log("changereadystate", state.readystate);
                },
                changegroups(state, payload) {
                    state.groups = payload.groups;
                    console.log("changegroups", state.groups);
                },
                changeonlineusers(state, payload) {
                    state.onlineusers = payload.onlineusers;
                    console.log("changeonlineusers", state.onlineusers);
                },
                changeallmessages(state, payload) {
                    state.allmessages = state.allmessages.concat(payload.messages);
                    console.log("changeallmessages", state.allmessages);
                }
            }
        });
        Vue.filter('grouptime', function (value) {
            if (!value) return '';
            return moment(value).format("ddd,HH:mm");
        });
        Vue.filter('timeago', function (value) {
            if (!value) return '';
            return moment(value).fromNow();
        });
        Vue.component('vue-group-item', {
            props: ['groupinfo'],
            data: function () {
                return {
                    unread: 0,
                    lstmsg: "",
                    lsttime: ""
                };
            },
            computed: {              
                allmessages: function () {
                    return this.$store.state.allmessages;
                }
            },
            template: "#groupItemTemplate",
            methods: {
                getUnread: function () {

                },
                getLastMsg: function () {

                }
            }
        });
        Vue.component('vue-groups-container', {
            template: "#groupsContainerTemplate",
            computed: {
                groups: function () {
                    return this.$store.state.groups;
                },
                allmessages: function () {
                    return this.$store.state.allmessages;
                }
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
            computed: {
                friends: function () {
                    return this.$store.state.onlineusers;
                }
            },
            template: "#friendsContainerTemplate",
            methods: {
                goChat: function (uid) {
                    var commanddto = { tguid: uid };
                    var bytes = serializeMsgPack(commanddto);
                    chat.sendCommand(chat.commandType.CreatePrivate, chat.dataType.MessagePack, bytes);
                }
            }
        });
        Vue.component('vue-allmessages-container', {
            template: "#allmessagesContainerTemplate",
            computed: {
                groups: function () {
                    return this.$store.state.groups;
                }
            }
        });
        Vue.component('vue-messages-container', {
            props: ['currentgroup', "loginuid"],
            template: "#messagesContainerTemplate",
            data: function () {
                return {
                    message: ""
                };
            },
            computed: {   
                readystate: function () {
                    return store.state.readystate;
                },
                messages: function () {
                    var rs = [];
                    for (var i = 0; i < this.$store.state.allmessages.length; i++) {
                        if (this.$store.state.allmessages[i].gid === this.currentgroup.gid) {
                            rs.push(this.$store.state.allmessages[i]);
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
                    if (this.$store.state.readystate !== 1) {
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
            store,
            computed: {
                readystate: function () {
                    return store.readystate;
                },
                groups: function () {
                    return store.groups;
                },
                onlineusers: function () {
                    return store.onlineusers;
                },
                allmessages: function () {
                    return store.allmessages;
                }
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
            store.commit('changereadystate', {
                readystate: 1
            });
            chatApp.getGroups();
        });
        abp.event.on("websocket.onconnecting", function (data) {
            store.commit('changereadystate', {
                readystate: 0
            });
        });
        abp.event.on("websocket.onclose", function (data) {
            store.commit('changereadystate', {
                readystate: 3
            });
        });
        abp.event.on("websocket.onerror", function (data) {
            console.log("onerror", event);
        });
        abp.event.on("websocket.onreceivebinary", function (data) {
            var cmddto = chat.receiveCommand(data);
            console.log("commandType", cmddto.commandType);
            if (cmddto.dataType === chat.dataType.MessagePack) {
                var decodedObj = deserializeMsgPack(cmddto.data);
                switch (cmddto.commandType) {
                    case chat.commandType.GetMessage:
                        store.commit('changeallmessages', {
                            messages: decodedObj
                        });
                        break;
                    case chat.commandType.GetGroups:
                        store.commit('changegroups', {
                            groups: decodedObj
                        });
                        break;
                    case chat.commandType.AlertMessage:
                        alert(decodedObj.text);
                        break;
                    case chat.commandType.GetOnlineUsers:
                        store.commit('changeonlineusers', {
                            onlineusers: decodedObj
                        });
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