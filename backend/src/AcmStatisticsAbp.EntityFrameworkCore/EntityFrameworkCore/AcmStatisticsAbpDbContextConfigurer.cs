// <copyright file="AcmStatisticsAbpDbContextConfigurer.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore
{
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;

    public static class AcmStatisticsAbpDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AcmStatisticsAbpDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AcmStatisticsAbpDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}
