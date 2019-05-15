using System.Web;

namespace JK.Cryptography
{
    public static class UrlHelper
    {
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }
    }
}
