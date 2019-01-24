using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace JK.Web.Views
{
    public abstract class JKRazorPage<TModel> : AbpRazorPage<TModel>
    {
        /// <summary>
        /// 当前页面名称
        /// </summary>
        public string CurrentPageName
        {
            set
            {
                ViewBag.CurrentPageName = value;
            }
            get
            {
                return ViewBag.CurrentPageName;
            }
        }
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected JKRazorPage()
        {
            LocalizationSourceName = JKConsts.LocalizationSourceName;
        }
    }
}
