// <copyright file="FeatureValueStore.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Features
{
    using Abp.Application.Features;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.MultiTenancy;
    using Abp.Runtime.Caching;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.MultiTenancy;

    public class FeatureValueStore : AbpFeatureValueStore<Tenant, User>
    {
        public FeatureValueStore(
            ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<EditionFeatureSetting, long> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                  cacheManager,
                  tenantFeatureRepository,
                  tenantRepository,
                  editionFeatureRepository,
                  featureManager,
                  unitOfWorkManager)
        {
        }
    }
}
