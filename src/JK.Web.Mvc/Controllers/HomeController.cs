using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using JK.Controllers;

namespace JK.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : JKControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
