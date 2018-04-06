// <copyright file="ExternalLoginProviderInfo.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authentication.External
{
    using System;

    public class ExternalLoginProviderInfo
    {
        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Type ProviderApiType { get; set; }

        public ExternalLoginProviderInfo(string name, string clientId, string clientSecret, Type providerApiType)
        {
            this.Name = name;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.ProviderApiType = providerApiType;
        }
    }
}
