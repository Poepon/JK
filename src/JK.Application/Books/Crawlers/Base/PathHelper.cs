namespace JK.Books.Crawlers
{
    public static class PathHelper
    {
        public static string GetAvailablePath(string value)
        {
            foreach (var item in "\\/:*?\"<>|".ToCharArray())
            {
                value = value.Replace(item.ToString(), string.Empty);
            }
            value = value.Replace("\t", "");
            return value;
        }
    }
}