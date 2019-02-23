using JK.Controllers;
using JK.Profile;
using JK.Profile.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JK.Web.Controllers
{
    public class ProfileController : JKControllerBase
    {
        private readonly IProfileAppService _profileAppService;

        public ProfileController(IProfileAppService profileAppService)
        {
            _profileAppService = profileAppService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SetTwoFactorLogin()
        {
            var model = await _profileAppService.GetTwoFactorLoginForEdit();
            return this.PartialView("SetTwoFactorLogin", model);
        }

        #region Change Password

        public IActionResult ChangePasswordModal()
        {
            var model = new ChangePasswordInput();
            return this.PartialView("ChangePassword", model);
        }

        #endregion
    }
}
