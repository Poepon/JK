using Abp.Domain.Repositories;
using Abp.IO;
using Abp.RabbitMQ.AutoSubscribe;
using JK.Books.Crawlers;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JK.Books.RabbitMQ
{
    public class BookCoverRabbitMQConsumer : RabbitMQConsumerBase<BookMessage>, IRabbitMQConsumer<BookMessage>
    {
        private readonly IAppFolders appFolders;
        private readonly IRepository<Book> repository;

        public BookCoverRabbitMQConsumer(IAppFolders appFolders, IRepository<Book> repository)
        {
            this.appFolders = appFolders;
            this.repository = repository;
        }
        public override async Task ConsumeAsync(BookMessage message)
        {
            if (string.IsNullOrEmpty(appFolders.BookRootPath))
            {
                throw new Exception("IAppFolders.BookRootPath 未初始化。");
            }
            var book = await repository.GetAll().Include(b => b.Author).SingleOrDefaultAsync(b => b.Id == message.BookId);
            if (book == null || book.UseLocalImage)
            {
                return;
            }
            using (var client = new WebClient())
            {
                var bookDirRelativePath = $"/BookContents/{PathHelper.GetAvailablePath(book.Author.Name)}/{book.Name}";
                string dir = appFolders.BookRootPath + bookDirRelativePath;
                DirectoryHelper.CreateIfNotExists(dir);

                string filePath = Path.Combine(dir, "cover.jpg");
                var data = client.DownloadData(book.ImageSrc);
                var encoder = new JpegEncoder()
                {
                    Quality = 50,
                    Subsample = JpegSubsample.Ratio420
                };
                var image = Image.Load(data);
                image.Save(filePath, encoder);
                book.UseLocalImage = true;
                await repository.UpdateAsync(book);
            }
        }
        public override string GetRoutingKey()
        {
            return "BookCover";
        }
    }
}
