using Abp.Events.Bus;

namespace JK.Books.Events
{
    public class CrawlChapterCompletedEventData : EventData
    {
        public CrawlChapterCompletedEventData(int bookId)
        {
            BookId = bookId;
        }
        public int BookId { get; set; }
    }
}
