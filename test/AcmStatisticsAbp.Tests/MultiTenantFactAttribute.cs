// <copyright file="MultiTenantFactAttribute.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests
{
    using Xunit;

    public sealed class MultiTenantFactAttribute : FactAttribute
    {
        public MultiTenantFactAttribute()
        {
            if (!AcmStatisticsAbpConsts.MultiTenancyEnabled)
            {
#pragma warning disable CS0162 // 检测到无法访问的代码
                this.Skip = "MultiTenancy is disabled.";
#pragma warning restore CS0162 // 检测到无法访问的代码
            }
        }
    }
}
