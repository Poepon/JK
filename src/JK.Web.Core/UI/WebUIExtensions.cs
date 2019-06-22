using Abp.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JK.UI
{
    public static class WebUIExtensions
    {
        public static IReadOnlyList<SelectListItem> ToSelectListItems(this Enum e, ILocalizationManager localizationManager = null, bool Selected = true)
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
