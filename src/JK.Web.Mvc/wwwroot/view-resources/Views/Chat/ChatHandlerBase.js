var ChatHandlerBase = /** @class */ (function () {
    function ChatHandlerBase() {
        this.littleEndian = function () {
            var buffer = new ArrayBuffer(2);
            new DataView(buffer).setInt16(0, 256, true);
            return new Int16Array(buffer)[0] === 256;
        };
    }
    return ChatHandlerBase;
}());
//# sourceMappingURL=ChatHandlerBase.js.map