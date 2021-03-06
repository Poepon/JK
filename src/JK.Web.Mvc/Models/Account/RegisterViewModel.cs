using Abp.Auditing;
using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace JK.Web.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool IsExternalLogin { get; set; }

        public string ExternalLoginAuthSchema { get; set; }

    }
}
