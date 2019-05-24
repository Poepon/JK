using System.Collections.Generic;
using Abp.Timing;
using JK.Logs.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JK.Web.Models.LoginLogs
{
    public class LoginLogViewModel : GetUserLoginAttemptsInput
    {
        public LoginLogViewModel()
        {
            this.StartTime = Clock.Now.Date;
            this.EndTime = Clock.Now.Date.AddDays(1).AddMilliseconds(-1);
        }
        public IReadOnlyList<SelectListItem> Tenants { get; set; }
    }
}
