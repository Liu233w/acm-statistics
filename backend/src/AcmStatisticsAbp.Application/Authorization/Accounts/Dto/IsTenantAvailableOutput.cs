// <copyright file="IsTenantAvailableOutput.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authorization.Accounts.Dto
{
    public class IsTenantAvailableOutput
    {
        public TenantAvailabilityState State { get; set; }

        public int? TenantId { get; set; }

        public IsTenantAvailableOutput()
        {
        }

        public IsTenantAvailableOutput(TenantAvailabilityState state, int? tenantId = null)
        {
            this.State = state;
            this.TenantId = tenantId;
        }
    }
}
