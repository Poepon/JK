using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.IO;
using Castle.Core.Logging;
using JK.Books.Crawlers;
using JK.Books.Interfaces;

namespace JK.Books.Implementation
{
    public class ContentCrawlerAppService : IContentCrawlerAppService
    {
        private readonly IAppFolders _appFolders;

        public ContentCrawlerAppService(IAppFolders appFolders)
        {
            _appFolders = appFolders;
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Logger = NullLogger.Instance;
        }

        public virtual ILogger Logger { get; set; }

        public async Task<(bool flag, string src)> CrawlContent(string authorName, string bookName, BookChapter bookChapter, bool hasSameTitle)
        {
            if (string.IsNullOrEmpty(_appFolders.BookRootPath))
            {
                throw new Exception("IAppFolders.BookRootPath 未初始化。");
            }
            bool hasSrc = false;
            bool flag = true;
            var bookDirRelativePath =
                $"/BookContents/{PathHelper.GetAvailablePath(authorName)}/{bookName}";
            var bookDirAbsolutePath = _appFolders.BookRootPath + bookDirRelativePath;
            var filename =
                $"{PathHelper.GetAvailablePath(bookChapter.Title)}{(hasSameTitle ? bookChapter.SourceChapterId.ToString() : "")}.html";
            string bookRelativePath = $"{bookDirRelativePath}/{filename}";
            DirectoryHelper.CreateIfNotExists(bookDirAbsolutePath);

            if (!hasSameTitle)
            {
                hasSrc = CopyFromLocalBackup(bookChapter, bookDirAbsolutePath, filename);
            }
            if (!hasSrc)
            {
                if (bookChapter.ChapterLinks != null)
                {
                    var crawlers = IocManager.Instance.ResolveAll<IContentCrawler>();

                    foreach (var item in bookChapter.ChapterLinks)
                    {
                        try
                        {
                            var crawler = crawlers.FirstOrDefault(c => c.Source == item.Source);
                            if (crawler == null || !item.IsActive)
                            {
                                continue;
                            }

                            var respone = await crawler.CrawlerChapterContent(item.Link);
                            if (respone.flag)
                            {
                                if (string.IsNullOrEmpty(respone.content))
                                {
                                    Logger.Error(item.Link + "解析失败。");
                                    continue;
                                }
                                if (!bookChapter.IsFree && respone.content.Length < 1000)
                                {
                                    Logger.Warn($"{bookChapter.Book.Name}-{bookChapter.Title}-{item.Source}-{item.Link}");
                                    item.IsActive = false;
                                    continue;
                                }
                                await File.WriteAllTextAsync(_appFolders.BookRootPath + bookRelativePath, respone.content, Encoding.UTF8);
                                hasSrc = true;
                            }
                            else if (item.Source == BookSource.QiDian)
                            {
                                flag = false;
                            }
                            break;
                        }
                        catch (AggregateException e)
                        {
                            if (e.InnerException is HttpRequestException)
                            {
                                Logger.Info(item.Link + "服务器禁止响应。" + e.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Info(item.Link + "爬内容失败。" + e.Message);
                        }
                    }
                }
                else
                {
                    Logger.Info("ChapterLinks为空。");
                }
            }
            return (flag, hasSrc ? bookRelativePath : "");
        }

        /// <summary>
        /// 复制自本地备份
        /// </summary>
        /// <param name="book"></param>
        /// <param name="bookChapter"></param>
        /// <param name="bookDirAbsolutePath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool CopyFromLocalBackup(
            BookChapter bookChapter,
            string bookDirAbsolutePath,
            string filename)
        {

            string path = null;
            string backupPath = null;
            try
            {
                path = Path.Combine(bookDirAbsolutePath, filename);
                backupPath = Path.Combine(_appFolders.BookRootPath + $"/BookContentsBackup/{bookChapter.Book.Name}", filename);
            }
            catch (Exception e)
            {
                throw e;
            }
            bool hasSrc = false;
            if (File.Exists(backupPath))
            {
                if (!bookChapter.IsFree && File.ReadAllText(backupPath).Length < 1000)
                {
                    File.Delete(backupPath);
                    Logger.Info("内容长度小于1000，删除备份文件：" + backupPath);
                }
                else
                {
                    File.Move(backupPath, path);
                }
            }
            if (File.Exists(path))
            {
                hasSrc = true;
            }
            return hasSrc;
        }
    }
}