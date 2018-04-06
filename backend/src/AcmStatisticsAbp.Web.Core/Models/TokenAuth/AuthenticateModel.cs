// <copyright file="AuthenticateModel.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Models.TokenAuth
{
    using System.ComponentModel.DataAnnotations;
    using Abp.Authorization.Users;

    public class AuthenticateModel
    {
        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        public string Password { get; set; }

        public bool RememberClient { get; set; }
    }
}
