using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AcmStatisticsBackend.Authorization.Users;

namespace AcmStatisticsBackend.Settings
{
    /// <summary>
    /// Setting attributes for each user.
    /// </summary>
    public class UserSettingAttribute : Entity<long>, IAudited<User>
    {
        [Required]
        public User User { get; set; }
        public long UserId { get; set; }

        /// <summary>
        /// The time (UTC) of last time zone changed.
        /// </summary>
        public DateTime? LastTimeZoneChangedTime { get; set; }

        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public User CreatorUser { get; set; }
        public User LastModifierUser { get; set; }
    }
}
