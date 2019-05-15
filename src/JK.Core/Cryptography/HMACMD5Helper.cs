using System;
using System.Security.Cryptography;
using System.Text;

namespace JK.Cryptography
{
    public static class HMACMD5Helper
    {

        public static string Encrypt(this string data, string key)
        {
            return Encrypt(data, key, Encoding.UTF8);
        }

        public static string Encrypt(this string data, string key, Encoding encoding)
        {
            using (var hash = new HMACMD5(encoding.GetBytes(key)))
            {
                return BitConverter.ToString(hash.ComputeHash(encoding.GetBytes(data))).Replace("-", "").ToLower();
            }
        }




    }
}
