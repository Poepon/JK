namespace JK.Web.Public.Models.Account
{
    public class LoginFormViewModel
    {

        public string ReturnUrl { get; set; }

        public bool IsMultiTenancyEnabled { get; set; }

        public bool IsSelfRegistrationAllowed { get; set; }
        
    }
}
