using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using JK.Controllers;
using JK.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using JK.Web.Models.LoginLogs;

namespace JK.Web.Controllers
{
    /// <summary>
    /// 登录日志
    /// </summary>
    [AbpMvcAuthorize]
    [DisableAuditing]
    public class LoginLogsController : JKControllerBase
    {
        private readonly TenantManager tenantManager;

        public LoginLogsController(TenantManager tenantManager)
        {
            this.tenantManager = tenantManager;
        }
        /// <summary>
        /// 登录日志
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var tenants = tenantManager.Tenants.ToList();
            var model = new LoginLogViewModel()
            {
                Tenants = tenants.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList()
            };
            return View(model);
        }
    }
}
