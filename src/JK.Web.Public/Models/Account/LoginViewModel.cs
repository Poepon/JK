using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace JK.Web.Public.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
