using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using JK.Controllers;
using Microsoft.AspNetCore.Mvc;
using Abp.Dependency;

namespace JK.Web.Controllers
{
    [DisableAuditing]
    [AbpMvcAuthorize]
    public class ChatController : JKControllerBase, ITransientDependency
    {
        public ChatController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }
    }
}
