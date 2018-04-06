// <copyright file="ExternalAuthConfiguration.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authentication.External
{
    using System.Collections.Generic;
    using Abp.Dependency;

    public class ExternalAuthConfiguration : IExternalAuthConfiguration, ISingletonDependency
    {
        public List<ExternalLoginProviderInfo> Providers { get; }

        public ExternalAuthConfiguration()
        {
            this.Providers = new List<ExternalLoginProviderInfo>();
        }
    }
}
