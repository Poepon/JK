using System;
using System.Text;

namespace JK.Chat
{
    public static class DateTimeExtensions
    {
        public static long ToUnix(this DateTime dt)
        {
            return (dt.Ticks  - 621355968000000000)/ 10000000;
        }
        public static DateTime ToDateTime(this long timestamp)
        {
            return new DateTime(timestamp* 10000000 + 621355968000000000);
        }
    }
    public static class BytesExtensions
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
