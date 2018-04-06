// <copyright file="ExternalAuthProviderApiBase.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authentication.External
{
    using System.Threading.Tasks;
    using Abp.Dependency;

    public abstract class ExternalAuthProviderApiBase : IExternalAuthProviderApi, ITransientDependency
    {
        public ExternalLoginProviderInfo ProviderInfo { get; set; }

        public void Initialize(ExternalLoginProviderInfo providerInfo)
        {
            this.ProviderInfo = providerInfo;
        }

        public async Task<bool> IsValidUser(string userId, string accessCode)
        {
            var userInfo = await this.GetUserInfo(accessCode);
            return userInfo.ProviderKey == userId;
        }

        public abstract Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);
    }
}
