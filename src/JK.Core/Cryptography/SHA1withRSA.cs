using System;
using System.Text;

namespace JK.Cryptography
{
    public class SHA1withRSA
    {
        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="content">数据</param>
        /// <param name="privateKey">RSA密钥</param>
        /// <returns></returns>
        public static string RasSign(string content, string privateKey)
        {
            throw new NotImplementedException();
            //var signer = SignerUtilities.GetSigner("SHA1withRSA");
            ////将java格式的rsa密钥转换成.net格式
            //var privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            //signer.Init(true, privateKeyParam);
            //var plainBytes = Encoding.UTF8.GetBytes(content);
            //signer.BlockUpdate(plainBytes, 0, plainBytes.Length);
            //var signBytes = signer.GenerateSignature();
            //return Convert.ToBase64String(signBytes);
        }


        /// <summary>
        /// RSA验签
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="publicKey">RSA公钥</param>
        /// <param name="signData">签名字段</param>
        /// <returns></returns>
        public static bool VerifySign(string content, string publicKey, string signData)
        {
            throw new NotImplementedException();
            //var signer = SignerUtilities.GetSigner("SHA1withRSA");
            //var publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            //signer.Init(false, publicKeyParam);
            //var signBytes = Convert.FromBase64String(signData.Replace(" ", "+"));
            //var plainBytes = Encoding.UTF8.GetBytes(content);
            //signer.BlockUpdate(plainBytes, 0, plainBytes.Length);
            //var ret = signer.VerifySignature(signBytes);
            //return ret;
        }

    }
}
