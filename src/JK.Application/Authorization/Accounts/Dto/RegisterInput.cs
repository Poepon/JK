using Abp.Auditing;
using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace JK.Authorization.Accounts.Dto
{
    public class RegisterInput
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }

    }
}
