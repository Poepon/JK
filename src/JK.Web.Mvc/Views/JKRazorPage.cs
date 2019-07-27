using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Abp.Localization;
using System.Linq;

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

        public IReadOnlyList<SelectListItem> EumnToSelectListItems(Enum e, ILocalizationManager localizationManager = null, bool Selected = true)
        {
            var type = e.GetType();
            return type.GetEnumNames().Select(n => new SelectListItem
            {
                Text = localizationManager != null
                    ? localizationManager.GetString(JKConsts.LocalizationSourceName, $"{type.Name}:{n}")
                    : n,
                Value = n,
                Selected = Selected && n == e.ToString()
            }).ToList();
        }
    }
}
