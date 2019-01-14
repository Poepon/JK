using Abp.AspNetCore.Mvc.ViewComponents;

namespace JK.Web.Views
{
    public abstract class JKViewComponent : AbpViewComponent
    {
        protected JKViewComponent()
        {
            LocalizationSourceName = JKConsts.LocalizationSourceName;
        }
    }
}
