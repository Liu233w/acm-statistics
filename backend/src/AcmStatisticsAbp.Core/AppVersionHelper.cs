// <copyright file="AppVersionHelper.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp
{
    using System;
    using System.IO;
    using Abp.Reflection.Extensions;

    /// <summary>
    /// Central point for application version.
    /// </summary>
    public class AppVersionHelper
    {
        /// <summary>
        /// Gets current version of the application.
        /// It's also shown in the web page.
        /// </summary>
        public const string Version = "3.5.0.0";

        /// <summary>
        /// Gets release (last build) date of the application.
        /// It's shown in the web page.
        /// </summary>
        public static DateTime ReleaseDate
        {
            get { return new FileInfo(typeof(AppVersionHelper).GetAssembly().Location).LastWriteTime; }
        }
    }
}
