using System.ComponentModel.DataAnnotations;

namespace JK.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}