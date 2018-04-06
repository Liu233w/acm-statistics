// <copyright file="AcmStatisticsAbpRepositoryBase.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single class

namespace AcmStatisticsAbp.EntityFrameworkCore.Repositories
{
    using Abp.Domain.Entities;
    using Abp.EntityFrameworkCore;
    using Abp.EntityFrameworkCore.Repositories;

    /// <summary>
    /// Base class for custom repositories of the application.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public abstract class AcmStatisticsAbpRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<AcmStatisticsAbpDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected AcmStatisticsAbpRepositoryBase(IDbContextProvider<AcmStatisticsAbpDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Add your common methods for all repositories
    }

    /// <summary>
    /// Base class for custom repositories of the application.
    /// This is a shortcut of <see cref="AcmStatisticsAbpRepositoryBase{TEntity,TPrimaryKey}"/> for <see cref="int"/> primary key.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class AcmStatisticsAbpRepositoryBase<TEntity> : AcmStatisticsAbpRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected AcmStatisticsAbpRepositoryBase(IDbContextProvider<AcmStatisticsAbpDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        // Do not add any method here, add to the class above (since this inherits it)!!!
    }
}
