using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JK.Books.Dto;

namespace JK.Books.Crawlers
{
    public abstract class CrawlerBase : IBookCrawler
    {
        protected readonly HttpClient _client;
        public CrawlerBase(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(Source);
        }
        public abstract string SearchUrlFormat { get; }

        public virtual string SearchMethod { get; } = "GET";

        public virtual string SearchPostKey { get; } = "";

        public abstract string Source { get; }

        public abstract string Host { get; }

        public abstract int DelaySecond { get; }

        public abstract int SkipItems { get; }

        public virtual Encoding ContentEncoding => Encoding.UTF8;

        public virtual Encoding UrlEncoding => ContentEncoding;

        public virtual Encoding ChapterLinkEncoding => ContentEncoding;

        public virtual async Task<string> CrawlerBookLinks(string bookName, string author)
        {
            HttpResponseMessage responseMessage = null;
            if (SearchMethod == "GET")
            {
                var tempUrlData = System.Web.HttpUtility.UrlEncode(bookName, UrlEncoding);
                string url = string.Format(SearchUrlFormat, tempUrlData);
                responseMessage = await _client.GetAsync(url);
            }
            else
            {
                var form = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {SearchPostKey,bookName }
                });
                responseMessage = await _client.PostAsync(SearchUrlFormat, form);
            }
            responseMessage.EnsureSuccessStatusCode();
            var stream = responseMessage.Content.ReadAsStreamAsync().Result;
            using (stream)
            {
                using (var reader = new StreamReader(stream, ContentEncoding))
                {
                    var html = reader.ReadToEnd();
                    var bookLink = AnalysisBookLink(bookName, author, responseMessage.RequestMessage.RequestUri.AbsoluteUri, html);
                    return bookLink;
                }
            }
        }

        public virtual async Task<(CreateBookDto, List<BookChapterLinkDto>)> CrawlerChapterLinks(string bookLink)
        {
            var respone = await _client.GetStringAsync(bookLink, ChapterLinkEncoding);

            var book = AnalysisChapterLinks(bookLink, respone.html);
            return book;
        }

        protected abstract (CreateBookDto, List<BookChapterLinkDto>) AnalysisChapterLinks(string bookLink, string html);


        protected abstract string AnalysisBookLink(string bookName, string author,
            string url, string html);
    }
}