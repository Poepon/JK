using Abp.Application.Services;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using JK.Books.Crawlers;
using JK.Books.Dto;
using JK.Books.Events;
using JK.Books.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Abp.Domain.Repositories;
using Abp.Timing;
using JK.IRepositories;

namespace JK.Books.QiDian
{
    [RemoteService(false)]
    public class ChapterCrawlerAppService : JKAppServiceBase, IChapterCrawlerAppService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookVolumeRepository _bookVolumeRepository;
        private readonly IBookChapterRepository _bookChapterRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public virtual IEventBus EventBus { get; set; }

        public ChapterCrawlerAppService(
            IBookRepository bookRepository,
            IBookVolumeRepository bookVolumeRepository,
            IBookChapterRepository bookChapterRepository,
            IHttpClientFactory httpClientFactory)
        {
            _bookRepository = bookRepository;
            _bookVolumeRepository = bookVolumeRepository;
            _bookChapterRepository = bookChapterRepository;
            _httpClientFactory = httpClientFactory;

            EventBus = NullEventBus.Instance;
        }

        private const string infoUrlFormat = "https://m.qidian.com/book/{0}";
        private const string catalogUrlFormat = "https://m.qidian.com/book/{0}/catalog";

        [UnitOfWork(false, IsDisabled = true)]
        public bool CrawlQiDianChapter(int bookId)
        {
            var book = _bookRepository.Get(bookId);
            var _httpClient = _httpClientFactory.CreateClient(BookSource.QiDian);
            //var infoUrl = string.Format(infoUrlFormat, book.SourceBookId);
            //var infoHtml = _httpClient.GetString(infoUrl, Encoding.UTF8);
            var catalogUrl = string.Format(catalogUrlFormat, book.SourceBookId);
            try
            {
                var respone = _httpClient.GetString(catalogUrl, Encoding.UTF8, false);
                if (respone.statusCode == 200)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(respone.html);
                    var node = doc.DocumentNode.SelectSingleNode("/html/body/script[8]");
                    var json = node.InnerHtml
                        .Substring(node.InnerHtml.IndexOf("g_data.volumes = ", StringComparison.Ordinal) +
                                   "g_data.volumes = ".Length).Trim();
                    json = json.Substring(0, json.Length - 1);
                    var data = JsonConvert.DeserializeObject<List<QiDianBookVolume>>(json);
                    var volumes = _bookVolumeRepository.GetAll().Where(v => v.BookId == book.Id).ToList();
                    var chapters = _bookChapterRepository.GetAll().Where(v => v.BookId == book.Id).ToList();
                    int chapterIndex = 0;
                    var deleteChapters = chapters.Where(c => data.All(v => v.cs.All(item => item.id != c.SourceChapterId)));
                    foreach (var item in deleteChapters)
                    {
                        item.IsActive = false;
                    }
                    for (int i = 0; i < data.Count; i++)
                    {
                        var qidianVolume = data[i];
                        var volume = volumes.FirstOrDefault(v => v.QiDianVolumeId == qidianVolume.vId);
                        if (volume == null)
                        {
                            volume = new BookVolume()
                            {
                                BookId = book.Id,
                                Name = qidianVolume.vN,
                                QiDianVolumeId = qidianVolume.vId,
                                IsVip = qidianVolume.vS,
                                Order = i + 1
                            };
                            volume = _bookVolumeRepository.Insert(volume);
                            CurrentUnitOfWork.SaveChanges();
                        }

                        if (volume != null && volume.Id > 0)
                        {
                            bool hasCreated = false;
                            for (int j = 0; j < qidianVolume.cs.Count; j++)
                            {
                                chapterIndex++;
                                var chapterItem = qidianVolume.cs[j];
                                var entity = new BookChapter()
                                {
                                    BookId = book.Id,
                                    Order = chapterIndex,
                                    ContentLenght = chapterItem.cnt,
                                    Source = BookSource.QiDian,
                                    SourceChapterId = chapterItem.id,
                                    Title = BookChapterHtmlHelper.GetChineseTitle(chapterItem.cN),
                                    IsFree = chapterItem.sS == 1,
                                    UpdateTime = chapterItem.uT.AddSeconds(-chapterItem.uT.Second),
                                    HasSrc = false,
                                    BookVolumeId = volume.Id,
                                };
                                var chapter = chapters.FirstOrDefault(c => c.SourceChapterId == entity.SourceChapterId);
                                if (chapter != null)
                                {
                                    var hasChange = false;
                                    if (chapter.Order != entity.Order)
                                    {
                                        chapter.Order = entity.Order;
                                        hasChange = true;
                                    }

                                    if (chapter.Title != entity.Title)
                                    {
                                        chapter.Title = entity.Title;
                                        hasChange = true;
                                    }

                                    if (hasChange)
                                    {
                                        _bookChapterRepository.Update(chapter);
                                        CurrentUnitOfWork.SaveChanges();
                                    }
                                }
                                else
                                {
                                    if (entity.IsFree)
                                    {
                                        var tempLink = $"https://m.qidian.com/book/{book.SourceBookId}/{chapterItem.id}";
                                        entity.ChapterLinks = new List<BookChapterLink>();

                                        var link = new BookChapterLink
                                        {
                                            BookChapterId = entity.Id,
                                            Link = tempLink,
                                            Source = BookSource.QiDian,
                                        };
                                        entity.ChapterLinks.Add(link);
                                    }
                                    _bookChapterRepository.Insert(entity);
                                    CurrentUnitOfWork.SaveChanges();
                                    if (hasCreated == false)
                                    {
                                        hasCreated = true;
                                    }
                                }
                            }
                            if (hasCreated)
                            {
                                EventBus.Trigger(new DeleteBookStaticFileEventData(book.Id));
                            }
                        }
                    }
                }
                else if(respone.statusCode.ToString().StartsWith("4"))
                {
                    book.IsActive = false;
                }
                book.LastCrawlDateTime = Clock.Now;
                _bookRepository.Update(book);
                CurrentUnitOfWork.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Warn($"CrawlQiDianChapter-{catalogUrl}：" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 章节分组
        /// </summary>
        class QiDianBookVolume
        {
            public long vId { get; set; }
            /// <summary>
            /// 章节数
            /// </summary>
            public int cCnt { get; set; }
            /// <summary>
            /// 是否VIP卷
            /// </summary>
            public bool vS { get; set; }

            public int isD { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string vN { get; set; }

            public List<QiDianBookChapterJsonData> cs { get; set; }
            /// <summary>
            /// 字数
            /// </summary>
            public int wC { get; set; }

            public bool hS { get; set; }

            public bool isLast { get; set; }
        }
        /// <summary>
        /// 章节明细
        /// </summary>
        class QiDianBookChapterJsonData
        {
            /// <summary>
            /// 排序
            /// </summary>
            public int uuid { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string cN { get; set; }
            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime uT { get; set; }
            /// <summary>
            /// 字数
            /// </summary>
            public int cnt { get; set; }

            public string cU { get; set; }
            /// <summary>
            /// ID
            /// </summary>
            public int id { get; set; }
            public int sS { get; set; }
            public int _y { get; set; }
            public bool isLast { get; set; }
        }

    }
}