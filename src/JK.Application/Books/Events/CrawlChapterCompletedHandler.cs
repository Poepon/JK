using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.Timing;
using Castle.Core.Logging;
using JK.Books.Interfaces;

namespace JK.Books.Events
{
    public class CrawlChapterCompletedHandler : IEventHandler<CrawlChapterCompletedEventData>, ITransientDependency
    {
        private readonly IBookAppService _bookAppService;
        private readonly ILogger _logger;

        public CrawlChapterCompletedHandler(IBookAppService bookAppService, ILogger logger)
        {
            _bookAppService = bookAppService;
            _logger = logger;
        }

        public void HandleEvent(CrawlChapterCompletedEventData eventData)
        {
            var b = _bookAppService.Get(new EntityDto(eventData.BookId));
            b.LastCrawlDateTime = Clock.Now;
            b.LastUpdateDateTime = _bookAppService.GetLastUpdateTime(b.Id);
            _bookAppService.Update(b);
            _logger.Info(eventData.BookId + "章节目录采集完成。");
        }
    }
}