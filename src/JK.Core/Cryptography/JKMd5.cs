using System;
using System.Security.Cryptography;
using System.Text;

namespace JK.Cryptography
{
    public class JKMd5
    {
        public static string GetMd5(string text)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
        public static string Get16BitMd5(string text)
        {
            var val = GetMd5(text);
            return val.Substring(8, 16);
        }

        public static string GetMd5ToLower(string text)
        {
            return GetMd5(text).ToLower();
        }

        public static string Get16BitMd5ToLower(string text)
        {
            return Get16BitMd5(text).ToLower();
        }
    }
}
