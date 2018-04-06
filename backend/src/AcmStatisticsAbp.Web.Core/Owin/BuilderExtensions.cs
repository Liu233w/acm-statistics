// <copyright file="BuilderExtensions.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

#if FEATURE_SIGNALR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Owin.Builder;
using Owin;

namespace AcmStatisticsAbp.Owin
{
    /// <summary>
    /// This class (UseAppBuilder method) integrates OWIN pipeline to ASP.NET Core pipeline and
    /// allows us to use Owin based middlewares in ASP.NET Core applications.
    /// </summary>
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseAppBuilder(
            this IApplicationBuilder app,
            Action<IAppBuilder> configure)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var appBuilder = new AppBuilder();
                    appBuilder.Properties["builder.DefaultApp"] = next;

                    configure(appBuilder);

                    return appBuilder.Build<Func<IDictionary<string, object>, Task>>();
                });
            });

            return app;
        }
    }
}
#endif
