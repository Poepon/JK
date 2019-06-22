using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace JK.Cryptography
{
    public static class SecurityHelper
    {
        private static readonly byte[] IvBytes = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 哈希加密算法
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns></returns>
        private static string HashEncrypt(HashAlgorithm hashAlgorithm, string input)
        {
            return HashEncrypt(hashAlgorithm, input, Encoding.UTF8);
        }
        /// <summary>
        /// 哈希加密算法
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns></returns>
        private static string HashEncrypt(HashAlgorithm hashAlgorithm, string input, Encoding encoding)
        {
            using (hashAlgorithm)
            {
                var data = hashAlgorithm.ComputeHash(encoding.GetBytes(input));
                return BitConverter.ToString(data).Replace("-", "");
            }
        }

        public static string HMACMD5(string value, string key)
        {
            return HashEncrypt(new HMACMD5(Encoding.UTF8.GetBytes(key)), value, Encoding.UTF8);
        }

        public static string HMACSHA256(string value, string key)
        {
            return HashEncrypt(new HMACSHA256(Encoding.UTF8.GetBytes(key)), value, Encoding.UTF8);
        }

        public static string MD5(string value, string key)
        {
            return HashEncrypt(System.Security.Cryptography.MD5.Create(), value + key, Encoding.UTF8);
        }

        public static string Base64Encode(string value)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(encbuff);
        }
        public static string Base64Decode(string value)
        {
            byte[] decbuff = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(decbuff);
        }

        public static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
        public static string UrlDecode(string value)
        {
            return HttpUtility.UrlDecode(value);
        }

        public static string AESEncrypt(string value, string key)
        {
            throw new NotImplementedException();
        }
        public static string AESDecrypt(string value, string key)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// DES 加密
        /// </summary>
        /// <param name="value"> 待加密的字符串 </param>
        /// <param name="key"> 密钥（8位） </param>
        /// <returns></returns>
        public static string DESEncrypt(string value, string key)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var des = Aes.Create();
                des.Mode = CipherMode.ECB; //兼容其他语言的 Des 加密算法
                des.Padding = PaddingMode.Zeros; //自动补 0

                using (var ms = new MemoryStream())
                {
                    var data = Encoding.UTF8.GetBytes(value);

                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(keyBytes, IvBytes), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="input"> 待解密的字符串 </param>
        /// <param name="key"> 密钥（8位） </param>
        /// <returns></returns>
        public static string DESDecrypt(string input, string key)
        {
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);

                var des = Aes.Create();
                des.Mode = CipherMode.ECB; //兼容其他语言的Des加密算法
                des.Padding = PaddingMode.Zeros; //自动补0

                using (var ms = new MemoryStream())
                {
                    var data = Convert.FromBase64String(input);

                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(keyBytes, IvBytes), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);

                        cs.FlushFinalBlock();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return input;
            }
        }

        public static string HMACSHA1(string input, string key)
        {
            byte[] secrectKey = Encoding.UTF8.GetBytes(key);
            return HashEncrypt(new HMACSHA1(secrectKey), input);
        }

        public static string RSAEncrypt(string value, string publicKey)
        {
            throw new NotImplementedException();
        }
        public static string RSADecrypt(string value, string privateKey)
        {
            throw new NotImplementedException();
        }
    }
}
