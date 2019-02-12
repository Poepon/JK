/// <reference path="../../../lib/abp-web-resources/abp/framework/scripts/abp.d.ts" />
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var MessagePackChatHandler = /** @class */ (function (_super) {
    __extends(MessagePackChatHandler, _super);
    function MessagePackChatHandler() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    MessagePackChatHandler.prototype.send = function (commandType, dataType, dataBytes) {
        var buffer = new ArrayBuffer(9 + dataBytes.length);
        var dv = new DataView(buffer);
        dv.setInt32(0, commandType, this.littleEndian());
        dv.setInt8(4, dataType);
        dv.setInt32(5, dataBytes.length, this.littleEndian());
        for (var i = 0; i < dataBytes.length; i++) {
            dv.setUint8(i + 9, dataBytes[i]);
        }
        abp.event.trigger("websocket.sendbinary", dv);
    };
    MessagePackChatHandler.prototype.sendMessage = function (message) {
        //var bytes = serializeMsgPack(message);
        throw new Error("Method not implemented.");
    };
    return MessagePackChatHandler;
}(ChatHandlerBase));
//# sourceMappingURL=MessagePackChatHandler.js.map