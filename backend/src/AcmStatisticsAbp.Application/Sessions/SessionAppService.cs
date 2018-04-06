// <copyright file="SessionAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Sessions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abp.Auditing;
    using AcmStatisticsAbp.Sessions.Dto;
    using AcmStatisticsAbp.SignalR;

    public class SessionAppService : AcmStatisticsAbpAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>
                    {
                        { "SignalR", SignalRFeature.IsAvailable },
                        { "SignalR.AspNetCore", SignalRFeature.IsAspNetCore },
                    },
                },
            };

            if (this.AbpSession.TenantId.HasValue)
            {
                output.Tenant = this.ObjectMapper.Map<TenantLoginInfoDto>(await this.GetCurrentTenantAsync());
            }

            if (this.AbpSession.UserId.HasValue)
            {
                output.User = this.ObjectMapper.Map<UserLoginInfoDto>(await this.GetCurrentUserAsync());
            }

            return output;
        }
    }
}
