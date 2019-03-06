using JK.MultiThemes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JK.MultiThemes
{
    public static class ServiceCollectionExtensions
    {
        public static void AddThemes(this IServiceCollection services,
            ThemeOptions themeOptions = null)
        {
            services.Configure<ThemeOptions>(options =>
            {
                options.StoreType = themeOptions?.StoreType;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IThemeProvider, ThemeProvider>();
            if (themeOptions != null && themeOptions.StoreType != null && typeof(IThemeConfigStore).IsAssignableFrom(themeOptions.StoreType))
            {
                services.AddSingleton(typeof(IThemeConfigStore), themeOptions.StoreType);
            }
            else
            {
                services.AddSingleton<IThemeConfigStore, NullThemeConfigStore>();
            }

            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander());
            });
        }
    }
}
