using Abp.AspNetCore.Mvc.Controllers;
using Abp.Extensions;
using JK.Web.Public.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JK.Web.Public.Controllers
{
    public class AccountController : AbpController
    {
        private readonly PublicSignInManager publicSignInManager;

        public AccountController(PublicSignInManager publicSignInManager)
        {
            this.publicSignInManager = publicSignInManager;
        }
        public async Task<ActionResult> Login()
        {
            return View();
        }

        public async Task<ActionResult> Logout(string returnUrl = "")
        {
            await publicSignInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = NormalizeReturnUrl(returnUrl);
                return Redirect(returnUrl);
            }

            return RedirectToAction("Login");
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "Home", new { area = "AppAreaName" });
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

    }
}
