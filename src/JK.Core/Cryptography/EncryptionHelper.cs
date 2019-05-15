using JK.Payments.Enumerates;
using JK.Payments.TenantConfigs;
using System.Text;

namespace JK.Cryptography
{
    public class EncryptionHelper
    {
        public static string GetEncryption(EncryptionMethod encryptionMethod, string value, ThirdPartyAccount account, string sign = "")
        {
            string returnValue = "";
            switch (encryptionMethod)
            {
                case EncryptionMethod.HMACMD5:
                    returnValue = HMACMD5Helper.Encrypt(value, account.MerchantKey);
                    break;
                case EncryptionMethod.Base64:
                    returnValue = Base64Helper.Base64Encrypt(value);
                    break;
                case EncryptionMethod.RSA:
                    //returnValue = Convert.ToBase64String(RSAHelper.RSAPublicKeySignByte(value, merchantConfig.PublicKey));
                    returnValue = new RSAHelper(RSAType.RSA, Encoding.UTF8, null, account.PublicKey).Encrypt(value, account.PublicKey);
                    break;
                case EncryptionMethod.SHA1withRSA:
                    returnValue = SHA1withRSA.RasSign(value, account.PrivateKey);
                    break;
                case EncryptionMethod.RSAVerify:
                    returnValue = SHA1withRSA.VerifySign(value, account.PublicKey, sign).ToString();
                    break;
                case EncryptionMethod.AES:
                    returnValue = AesHelper.AESEncrypt1(value, account.PublicKey);
                    break;
                case EncryptionMethod.DES:
                    returnValue = AesHelper.AESDecrypt1(value, account.PublicKey);
                    break;
                case EncryptionMethod.Base64Dec:
                    returnValue = Base64Helper.Base64Decrypt(value);
                    break;
                case EncryptionMethod.UrlEncode:
                    returnValue = UrlHelper.UrlEncode(value);
                    break;
                case EncryptionMethod.UrlEncodeDec:
                    returnValue = UrlHelper.UrlDecode(value);
                    break;
                case EncryptionMethod.MD5Has:
                    returnValue = JKMd5.GetMd5(value).ToUpper();
                    break;
                default:
                    returnValue = JKMd5.GetMd5(value);
                    break;
            }

            //TODO
            return returnValue;
        }
    }
}
