﻿using System.Net.Http;
using Abp.BackgroundJobs;
using Abp.Threading.Timers;
using JK.Books.Interfaces;

namespace JK.Books.Workers.QiDian
{
    /// <summary>
    /// 连载签约小说爬虫
    /// </summary>
    public class QiDianBookIngCreateWorker : QiDianBookCrawlerWorker
    {
        public QiDianBookIngCreateWorker(AbpTimer timer,
            IBookAppService bookAppService,
            IBookLinkAppService bookLinkAppService,
            IHttpClientFactory httpClientFactory,
            IBackgroundJobManager backgroundJobManager) :
            base(timer, bookAppService, bookLinkAppService, httpClientFactory, backgroundJobManager)
        {
        }

        protected override 排序方式 排序 { get; set; } = 排序方式.人气排序;

        protected override BookStatus? 完本状态 { get; set; } = BookStatus.Serializing;

        protected override 显示方式 显示 { get; set; } = 显示方式.栅格方式;

        protected override 更新时间 最后更新 { get; set; } = 更新时间.全部;

        public override int CheckPeriodAsMilliseconds => 1000 * 60 * 5;
    }
}