namespace JK.Payments.Enumerates
{
    public enum EncryptionMethod
    {
        No = 0,
        MD5Has = 1,
        RSA = 2,
        AES = 3,
        DES = 4,
        Hash = 5,
        Base64 = 6,
        UrlEncode = 7,
        SHA1 = 8,
        HMACMD5 = 9,
        HMACSHA1 = 10,
        HMACSHA256 = 11,
        HMACSHA512 = 12,
        Base64Dec = 13,
        UrlEncodeDec = 14,
        MD5 = 15,
        Md5Encrypt = 16,
        SHA1withRSA = 17,
        RSAVerify = 18


        //TODO Anything else? 
    }
}
