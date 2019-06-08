using Abp.Dependency;
using JK.Books;
using System.Text;
using WebMarkupMin.Core;
namespace JK.Web.Books.WebMarkupMin
{
    public class HtmlCompress : IHtmlCompress, ISingletonDependency
    {
        private readonly HtmlMinifier minifier;
        public HtmlCompress()
        {
            minifier = new HtmlMinifier();
        }
        public string Compress(string html)
        {
            return minifier.Minify(html, encoding: Encoding.UTF8).MinifiedContent;
        }
    }
}
