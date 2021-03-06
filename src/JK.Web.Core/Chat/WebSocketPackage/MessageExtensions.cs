﻿using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using Abp.Json;
using JK.Chat.Dto;
using JK.Chat.Dto.Input;
using MessagePack;

namespace JK.Chat.WebSocketPackage
{
    public static class MessageExtensions
    {
        public static T DeserializeFromBytes<T>(this BinaryMessage message) where T : class
        {
            var dataType = message.DataType;
            var data = message.Data;
            T value = default(T);
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = Encoding.UTF8.GetString(data) as T;
                    break;
                case MessageDataType.Json:
                    var jsonString = Encoding.UTF8.GetString(data);
                    value = jsonString.FromJsonString<T>();
                    break;
                case MessageDataType.MessagePack:
                    value = MessagePackSerializer.Deserialize<T>(data);
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(data, 0, data.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        value = ProtoBuf.Serializer.Deserialize<T>(ms);
                    }
                    break;
                case MessageDataType.Blob:
                    MemoryStream memoryStream = new MemoryStream(data);
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    var fileType = (FileType)binaryReader.ReadInt32();
                    //TODO 图片、音视频文件处理
                    break;
                default:
                    throw new NotSupportedException("不支持的数据格式。" + dataType.ToString());
            }
            return value;
        }

        public static byte[] SerializeToBytes<T>(this T obj, MessageDataType dataType)
        {
            byte[] bytes = null;
            switch (dataType)
            {
                case MessageDataType.Text:
                    bytes = obj.ToString().ToBytes();
                    break;
                case MessageDataType.Json:
                    bytes = obj.ToJsonString().ToBytes();
                    break;
                case MessageDataType.MessagePack:
                    bytes = MessagePackSerializer.Serialize(obj);
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        bytes = ms.ToArray();
                    }
                    break;
                case MessageDataType.Blob:

                    break;
                default:
                    break;
            }
            return bytes;
        }

        public static T DeserializeFromText<T>(this TextMessage message) where T : class
        {
            var dataType = message.DataType;
            var data = message.Data;
            T value = default(T);
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = data as T;
                    break;
                case MessageDataType.Json:
                    value = data.FromJsonString<T>();
                    break;
                case MessageDataType.MessagePack:
                case MessageDataType.Protobuf:
                case MessageDataType.Blob:
                default:
                    throw new NotSupportedException("不支持的数据格式。" + dataType.ToString());
            }
            return value;
        }

        public static string SerializeToText<T>(this T obj, MessageDataType dataType)
        {
            //TODO
            string value = "";
            switch (dataType)
            {
                case MessageDataType.Text:
                    value = obj.ToString();
                    break;
                case MessageDataType.Json:
                    value = obj.ToJsonString();
                    break;
                case MessageDataType.MessagePack:
                    var bytes = MessagePackSerializer.Serialize(obj);
                    value = bytes.GetString();
                    break;
                case MessageDataType.Protobuf:
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        bytes = ms.ToArray();
                        value = bytes.GetString();
                    }
                    break;
                case MessageDataType.Blob:
                    break;
                default:
                    break;
            }
            return value;
        }

        public static byte[] WrapPackage<T>(this T obj, WebSocketMessageType messageType, MessageDataType dataType, CommandType commandType)
        {
            if (messageType == WebSocketMessageType.Binary)
            {
                byte[] msgdata = obj.SerializeToBytes<T>(dataType);
                var packagedata = new byte[msgdata.Length + 9];
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32(commandType)), 0, packagedata, 0, 4);
                Array.Copy(BitConverter.GetBytes(Convert.ToByte(dataType)), 0, packagedata, 4, 1);
                Array.Copy(BitConverter.GetBytes(Convert.ToInt64(msgdata.Length)), 0, packagedata, 5, 4);
                Array.Copy(msgdata, 0, packagedata, 9, msgdata.Length);
                return packagedata;
            }
            else if (messageType == WebSocketMessageType.Text)
            {
                var message = new TextMessage
                {
                    CommandType = commandType,
                    DataType = dataType,
                    Data = obj.SerializeToText(dataType)
                };
                var packagedata = message.ToJsonString().ToBytes();
                return packagedata;
            }
            else
            {
                throw new NotSupportedException("WebSocketMessageType. Close is not supported.");
            }
        }

    }
}