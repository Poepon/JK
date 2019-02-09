using System.Text;

namespace JK.Chat
{
    public static class BytesExtensions
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string ToString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
