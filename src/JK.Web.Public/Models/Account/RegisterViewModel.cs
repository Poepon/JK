using Abp.Auditing;
using JK.Customers;
using System.ComponentModel.DataAnnotations;

namespace JK.Web.Public.Models.Account
{
    public class RegisterViewModel
    {

        [StringLength(Customer.MaxUserNameLength)]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [StringLength(Customer.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsExternalLogin { get; set; }

        public string ExternalLoginAuthSchema { get; set; }

    }
}
