// <copyright file="DefaultSettingsCreator.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore.Seed.Host
{
    using System.Linq;
    using Abp.Configuration;
    using Abp.Localization;
    using Abp.Net.Mail;
    using Microsoft.EntityFrameworkCore;

    public class DefaultSettingsCreator
    {
        private readonly AcmStatisticsAbpDbContext context;

        public DefaultSettingsCreator(AcmStatisticsAbpDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            // Emailing
            this.AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            this.AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");

            // Languages
            this.AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (this.context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            this.context.Settings.Add(new Setting(tenantId, null, name, value));
            this.context.SaveChanges();
        }
    }
}
