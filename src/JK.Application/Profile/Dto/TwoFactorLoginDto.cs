namespace JK.Profile.Dto
{
    public class TwoFactorLoginDto
    {
        public bool IsTwoFactorEnabled { get; set; }

        public string QrCodeSetupImageUrl { get; set; }

        public bool IsGoogleAuthenticatorEnabled { get; set; }
    }
}
