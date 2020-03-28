using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;

namespace AcmStatisticsBackend.Authorization.Accounts.Dto
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

        [Required]
        public string CaptchaText { get; set; }

        [Required]
        public string CaptchaId { get; set; }
    }
}
