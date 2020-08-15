using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OHunt.Web.Utils
{
    /// <summary>
    /// Add odata parameter to swagger docs
    /// </summary>
    public class QueryParameterFilter : IOperationFilter
    {
        // from https://stackoverflow.com/a/31419132
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;
            if (apiDescription.SupportedResponseTypes == null
                || apiDescription.SupportedResponseTypes.All(it => it.Type.Name != "IQueryable`1"))
            {
                return;
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "$top", "The max number of records" },
                { "$skip", "The number of records to skip" },
                { "$filter", "A function that must evaluate to true for a record to be returned" },
                { "$select", "Specifies a subset of properties to return" },
                { "$orderby", "Determines what values are used to order a collection of records" },
                {
                    "$count",
                    "Should the item be counted. Use $count=true&$top=0 to count entities while not really request them."
                }
            };

            foreach (var (key, value) in parameters)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = key,
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" },
                    In = ParameterLocation.Query,
                    Description = value,
                });
            }
        }
    }
}
