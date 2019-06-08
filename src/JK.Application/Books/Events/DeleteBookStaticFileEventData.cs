using Abp.Events.Bus;

namespace JK.Books.Events
{
    public class DeleteBookStaticFileEventData : EventData
    {
        public DeleteBookStaticFileEventData(int bookId)
        {
            this.BookId = bookId;
        }
        public int BookId { get; set; }
    }
}