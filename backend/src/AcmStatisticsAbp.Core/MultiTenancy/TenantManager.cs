// <copyright file="TenantManager.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.MultiTenancy
{
    using Abp.Application.Features;
    using Abp.Domain.Repositories;
    using Abp.MultiTenancy;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Editions;

    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IAbpZeroFeatureValueStore featureValueStore)
            : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore)
        {
        }
    }
}
