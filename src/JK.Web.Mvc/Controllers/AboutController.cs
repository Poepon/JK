using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using JK.Controllers;

namespace JK.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : JKControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}
