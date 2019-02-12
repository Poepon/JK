/// <reference path="../../../lib/abp-web-resources/abp/framework/scripts/abp.d.ts" />

class MessagePackChatHandler extends ChatHandlerBase {

    send(commandType, dataType, dataBytes): void {

        var buffer = new ArrayBuffer(9 + dataBytes.length);
        var dv = new DataView(buffer);
        dv.setInt32(0, commandType, this.littleEndian());
        dv.setInt8(4, dataType);
        dv.setInt32(5, dataBytes.length, this.littleEndian());
        for (var i = 0; i < dataBytes.length; i++) {
            dv.setUint8(i + 9, dataBytes[i]);
        }
        abp.event.trigger("websocket.sendbinary", dv);
    }

    sendMessage(message: SendMessageDto): void {
        //var bytes = serializeMsgPack(message);
        throw new Error("Method not implemented.");
    }
}