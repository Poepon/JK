using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.RabbitMQ.AutoSubscribe;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.RabbitMQ;

namespace JK.Books.RabbitMQ
{
    public class BookLinkRabbitMQConsumer : RabbitMQConsumerBase<BookMessage>, IRabbitMQConsumer<BookMessage>
    {
        private readonly IRepository<Book> repository;
        private readonly IRepository<BookLink> bookLinkRepository;
        private readonly IRepository<NotFoundBook> notFoundBookRepository;
        private readonly IRabbitMQProducer rabbitMQProducer;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public BookLinkRabbitMQConsumer(IRepository<Book> repository,
            IRepository<BookLink> bookLinkRepository,
            IRepository<NotFoundBook> notFoundBookRepository,
            IRabbitMQProducer rabbitMQProducer,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.repository = repository;
            this.bookLinkRepository = bookLinkRepository;
            this.notFoundBookRepository = notFoundBookRepository;
            this.rabbitMQProducer = rabbitMQProducer;
            this.unitOfWorkManager = unitOfWorkManager;
        }
        public override async Task ConsumeAsync(BookMessage message)
        {
            var crawlers = IocManager.Instance.ResolveAll<IBookCrawler>();
            foreach (var item in crawlers)
            {

                if (!await bookLinkRepository.GetAll().AnyAsync(link => link.BookId == message.BookId && link.Source == item.Source))
                {
                    var bookItem = await repository.GetAllIncluding(b => b.Author).Where(b => b.Id == message.BookId)
                   .Select(b => new { b.Name, AuthorName = b.Author.Name }).FirstOrDefaultAsync();
                    var bookUrl = await item.CrawlerBookLinks(bookItem.Name, bookItem.AuthorName);

                    if (!string.IsNullOrEmpty(bookUrl))
                    {
                        using (var unitOfWork = unitOfWorkManager.Begin())
                        {
                            var dto = bookLinkRepository.InsertAsync(new BookLink()
                            {
                                BookId = message.BookId,
                                Link = bookUrl,
                                Source = item.Source,
                                LastAccessTime = new DateTime(2000, 1, 1)
                            });
                            await unitOfWork.CompleteAsync();
                            //rabbitMQProducer.PublishAsync(new ExchangeDeclareConfiguration())
                            //_backgroundJobManager.Enqueue<ChapterLinkCrawlJob, ChapterLinkCrawlJobArgs>(new ChapterLinkCrawlJobArgs
                            //{
                            //    BookId = bookItem.Id,
                            //    BookLinkId = dto.Id
                            //});
                        }
                    }
                    else
                    {
                        await notFoundBookRepository.InsertAsync(new NotFoundBook() { BookId = message.BookId, Source = item.Source });
                    }
                }
            }
        }


        public override string GetRoutingKey()
        {
            return "BookLink";
        }
    }
}
