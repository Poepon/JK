using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.IO;
using Abp.RabbitMQ.AutoSubscribe;
using JK.Books.Crawlers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace JK.Books.RabbitMQ
{
    public class ContentRabbitMQConsumer : IRabbitMQConsumer<ChapterMessage>
    {
        private readonly IRepository<BookChapter> repository;
        private readonly IAppFolders appFolders;

        public ContentRabbitMQConsumer(IRepository<BookChapter> repository, IAppFolders appFolders)
        {
            this.repository = repository;
            this.appFolders = appFolders;
        }
        public async Task ConsumeAsync(ChapterMessage message)
        {

            var crawlers = IocManager.Instance.ResolveAll<IContentCrawler>();
            var result = await crawlers.First(c => c.Source == message.Source).CrawlerChapterContent(message.Url);
            if (result.flag)
            {
                //TODO 
                var authorName = "";
                var bookName = "";
                var chapter = repository.Get(message.ChapterId);
                var bookDirRelativePath =
             $"/BookContents/{PathHelper.GetAvailablePath(authorName)}/{bookName}";
                var bookDirAbsolutePath = appFolders.BookRootPath + bookDirRelativePath;
                var filename =
                    $"{PathHelper.GetAvailablePath(chapter.Title)}{chapter.SourceChapterId}.html";
                string bookRelativePath = $"{bookDirRelativePath}/{filename}";
                DirectoryHelper.CreateIfNotExists(bookDirAbsolutePath);
                await File.WriteAllTextAsync(appFolders.BookRootPath + bookRelativePath, result.content, Encoding.UTF8);
                chapter.HasSrc = true;
                chapter.Src = bookRelativePath;
                await repository.UpdateAsync(chapter);

            }
        }

        public string GetConnectionName()
        {
            return null;
        }

        public ExchangeDeclareConfiguration GetExchangeDeclare()
        {
            return new ExchangeDeclareConfiguration(typeof(BookMessage).FullName, "direct", true, false);
        }

        public Type GetMessageType()
        {
            return typeof(BookMessage);
        }

        public QOSConfiguration GetQOSConfiguration()
        {
            return new QOSConfiguration(0, false);
        }

        public QueueDeclareConfiguration GetQueueDeclare()
        {
            return new QueueDeclareConfiguration(this.GetType().FullName, true);
        }

        public string GetRoutingKey()
        {
            return "#";
        }
    }
}
