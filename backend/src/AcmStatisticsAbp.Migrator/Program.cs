// <copyright file="Program.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Migrator
{
    using System;
    using Abp;
    using Abp.Castle.Logging.Log4Net;
    using Abp.Collections.Extensions;
    using Abp.Dependency;
    using Castle.Facilities.Logging;

    public class Program
    {
        private static bool quietMode;

        public static void Main(string[] args)
        {
            ParseArgs(args);

            using (var bootstrapper = AbpBootstrapper.Create<AcmStatisticsAbpMigratorModule>())
            {
                bootstrapper.IocManager.IocContainer
                    .AddFacility<LoggingFacility>(
                        f => f.UseAbpLog4Net().WithConfig("log4net.config"));

                bootstrapper.Initialize();

                using (var migrateExecuter = bootstrapper.IocManager.ResolveAsDisposable<MultiTenantMigrateExecuter>())
                {
                    var migrationSucceeded = migrateExecuter.Object.Run(quietMode);

                    if (quietMode)
                    {
                        // exit clean (with exit code 0) if migration is a success, otherwise exit with code 1
                        var exitCode = Convert.ToInt32(!migrationSucceeded);
                        Environment.Exit(exitCode);
                    }
                    else
                    {
                        Console.WriteLine("Press ENTER to exit...");
                        Console.ReadLine();
                    }
                }
            }
        }

        private static void ParseArgs(string[] args)
        {
            if (args.IsNullOrEmpty())
            {
                return;
            }

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-q":
                        quietMode = true;
                        break;
                }
            }
        }
    }
}
