// <copyright file="InitialHostDbBuilder.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly AcmStatisticsAbpDbContext context;

        public InitialHostDbBuilder(AcmStatisticsAbpDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(this.context).Create();
            new DefaultLanguagesCreator(this.context).Create();
            new HostRoleAndUserCreator(this.context).Create();
            new DefaultSettingsCreator(this.context).Create();

            this.context.SaveChanges();
        }
    }
}
