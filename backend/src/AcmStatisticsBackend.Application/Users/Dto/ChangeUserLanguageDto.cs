using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
