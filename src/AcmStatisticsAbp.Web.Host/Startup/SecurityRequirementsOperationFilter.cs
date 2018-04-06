// <copyright file="SecurityRequirementsOperationFilter.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Web.Host.Startup
{
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Authorization;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var actionAttrs = context.ApiDescription.ActionAttributes();
            if (actionAttrs.OfType<AbpAllowAnonymousAttribute>().Any())
            {
                return;
            }

            var controllerAttrs = context.ApiDescription.ControllerAttributes();
            var actionAbpAuthorizeAttrs = actionAttrs.OfType<AbpAuthorizeAttribute>();

            if (!actionAbpAuthorizeAttrs.Any() && controllerAttrs.OfType<AbpAllowAnonymousAttribute>().Any())
            {
                return;
            }

            var controllerAbpAuthorizeAttrs = controllerAttrs.OfType<AbpAuthorizeAttribute>();
            if (controllerAbpAuthorizeAttrs.Any() || actionAbpAuthorizeAttrs.Any())
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });

                var permissions = controllerAbpAuthorizeAttrs.Union(actionAbpAuthorizeAttrs)
                    .SelectMany(p => p.Permissions)
                    .Distinct();

                if (permissions.Any())
                {
                    operation.Responses.Add("403", new Response { Description = "Forbidden" });
                }

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "bearerAuth", permissions },
                    },
                };
            }
        }
    }
}
