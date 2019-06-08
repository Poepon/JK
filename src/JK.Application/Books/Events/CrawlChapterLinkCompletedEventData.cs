using Abp.Events.Bus;
using JK.Books.Dto;

namespace JK.Books.Events
{
    public class CrawlChapterLinkCompletedEventData : EventData
    {
        public CrawlChapterLinkCompletedEventData(BookLinkDto bookLink)
        {
            BookLink = bookLink;
        }
        public BookLinkDto BookLink { get; set; }
    }
}