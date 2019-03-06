using Abp.AspNetCore.Mvc.Controllers;
using JK.MultiThemes;
using Microsoft.AspNetCore.Mvc;

namespace JK.Web.Public.Controllers
{
    public class HomeController : AbpController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ChangeTheme(string themeName)
        {
            var p = HttpContext.RequestServices.GetService(typeof(IThemeProvider)) as IThemeProvider;
            p.SetWorkingTheme(HttpContext.Request.Host.Host, themeName);
            return RedirectToAction("Index");
        }

    }
}
