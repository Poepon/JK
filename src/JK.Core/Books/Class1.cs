using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace JK.Books
{
    public class BookAnalysis : Entity
    {
        public int BookSiteId { get; set; }

        [ForeignKey(nameof(BookSiteId))]
        public virtual BookSite BookSite { get; set; }

        public string NameExp { get; set; }

        public string AuthorExp { get; set; }
    }

    public class VolumeAnalysis : Entity
    {
        public int BookSiteId { get; set; }

        [ForeignKey(nameof(BookSiteId))]
        public virtual BookSite BookSite { get; set; }

        public string NameExp { get; set; }
    }
    public class ChapterAnalysis : Entity
    {
        public int BookSiteId { get; set; }

        [ForeignKey(nameof(BookSiteId))]
        public virtual BookSite BookSite { get; set; }

        public string NameExp { get; set; }
    }

    public class ContentAnalysis
    {
        public int BookSiteId { get; set; }

        [ForeignKey(nameof(BookSiteId))]
        public virtual BookSite BookSite { get; set; }

        public string ContentExp { get; set; }
    }
}
