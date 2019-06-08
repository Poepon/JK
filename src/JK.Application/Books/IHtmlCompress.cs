namespace JK.Books
{
    public interface IHtmlCompress
    {
        string Compress(string html);
    }
    public class NullHtmlCompress : IHtmlCompress
    {
        public string Compress(string html)
        {
            return html;
        }
    }
}
