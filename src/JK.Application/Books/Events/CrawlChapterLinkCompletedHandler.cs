using System;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using JK.Books.Interfaces;

namespace JK.Books.Events
{
    public class CrawlChapterLinkCompletedHandler : IEventHandler<CrawlChapterLinkCompletedEventData>, ITransientDependency
    {
        private readonly IBookLinkAppService _bookLinkAppService;
        private readonly ILogger _logger;
        private readonly IBookChapterLinkAppService _bookChapterLinkAppService;

        public CrawlChapterLinkCompletedHandler(
            IBookLinkAppService bookLinkAppService,
            IBookChapterLinkAppService bookChapterLinkAppService,
            ILogger logger)
        {
            _bookLinkAppService = bookLinkAppService;
            _logger = logger;
            _bookChapterLinkAppService = bookChapterLinkAppService;
        }
        public void HandleEvent(CrawlChapterLinkCompletedEventData eventData)
        {
            var dto = eventData.BookLink;
            dto.LastAccessTime = _bookChapterLinkAppService.GetMaxDateTime(dto.BookId, dto.Source);
            _bookLinkAppService.Update(dto);
            _logger.Info($"采集BookId:{dto.BookId}[{dto.Source}]成功。");
        }
    }
}