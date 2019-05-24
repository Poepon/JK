using Abp.AspNetCore.Mvc.Controllers;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Web.Models;
using JK.Customers;
using JK.Web.Public.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using JK.Identity;
using CustomerLoginResult = JK.Identity.JKLoginResult<JK.Customers.Customer, JK.Customers.CustomerLogin, JK.Customers.CustomerClaim, JK.Customers.CustomerToken>;

namespace JK.Web.Public.Controllers
{
    public class AccountController : AbpController
    {
        private readonly CustomerManager _userManager;
        private readonly ITenantCache _tenantCache;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly CustomerLogInManager _logInManager;

        public AccountController(
            IMultiTenancyConfig multiTenancyConfig,
            ITenantCache tenantCache,
            CustomerManager userManager,
            SignInManager<Customer> signInManager,
            CustomerLogInManager logInManager)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _tenantCache = tenantCache;
            _signInManager = signInManager;
            _userManager = userManager;
            _logInManager = logInManager;
        }


        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                var loginResult = await GetLoginResultAsync(loginModel.Username, loginModel.Password, GetTenancyNameOrNull());
               if (loginResult.Result == AbpLoginResultType.Success)
                {
                    await _signInManager.SignInAsync(loginResult.User, loginModel.RememberMe);
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    return RedirectToLocal(returnUrl);
                }
                
                if (loginResult.Result== AbpLoginResultType.LockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginModel);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(loginModel);
        }


        private async Task<CustomerLoginResult> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password,AbpSession.TenantId.GetValueOrDefault(1));

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw new Exception("Login Failed.");
            }
        }


        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new Customer
                {
                    UserName = model.UserName,
                    IsActive=true, 
                    TenantId = 1
                };
                await _userManager.InitializeOptionsAsync(AbpSession.TenantId.GetValueOrDefault(1));
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "Home", new { area = "AppAreaName" });
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
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
        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }
        private bool IsSelfRegistrationEnabled()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return false; // No registration enabled for host users!
            }

            return true;
        }
    }
}
