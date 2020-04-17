using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Accounts.Dto
{
    public class ChangePasswordInput
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
