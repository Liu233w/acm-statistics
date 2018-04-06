// <copyright file="Log.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Migrator
{
    using System;
    using Abp.Dependency;
    using Abp.Timing;
    using Castle.Core.Logging;

    public class Log : ITransientDependency
    {
        public ILogger Logger { get; set; }

        public Log()
        {
            this.Logger = NullLogger.Instance;
        }

        public void Write(string text)
        {
            Console.WriteLine(Clock.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + text);
            this.Logger.Info(text);
        }
    }
}
