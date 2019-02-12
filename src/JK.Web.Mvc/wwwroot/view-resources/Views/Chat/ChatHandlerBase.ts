abstract class ChatHandlerBase {

    abstract send(commandType, dataType, dataBytes): void;

    abstract sendMessage(message: SendMessageDto): void;

    littleEndian = function () {
        var buffer = new ArrayBuffer(2);
        new DataView(buffer).setInt16(0, 256, true);
        return new Int16Array(buffer)[0] === 256;
    };
}
interface SendMessageDto {
    gid: number;
    uid: number;
    msg: string;
}
