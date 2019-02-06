using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using JK.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JK.Web.Controllers
{
    [DisableAuditing]
    [AbpMvcAuthorize()]
    public class ChatController : JKControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
