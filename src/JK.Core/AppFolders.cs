using Abp.Dependency;

namespace JK
{
    public class WorkerConfig
    {
        public bool BookLinkWorkerEnable { get; set; }

        public int BookLinkWorkerPeriod { get; set; }

        public bool ChapterLinkWorkerEnable { get; set; }

        public int ChapterLinkWorkerPeriod { get; set; }

        public bool ChapterContentWorkerEnable { get; set; }

        public int ChapterContentWorkerPeriod { get; set; }
    }
    public interface IAppFolders
    {

        string WebLogsFolder { get; set; }

        string WebRootPath { get; set; }

        string ContentRootPath { get; set; }

        string BookRootPath { get; set; }

    }
    public class AppFolders : IAppFolders, ISingletonDependency
    {

        public string WebLogsFolder { get; set; }

        public string WebRootPath { get; set; }

        public string ContentRootPath { get; set; }

        public string BookRootPath { get; set; }
    }
}
