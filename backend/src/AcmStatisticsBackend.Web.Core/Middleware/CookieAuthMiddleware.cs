using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AcmStatisticsBackend.Middleware
{
    /// <summary>
    /// 如果 header 中没有 authorization，就从cookie中取到并放进header中
    /// </summary>
    public class CookieAuthMiddleware
    {
        private const string CookieAuthKey = "OAuthToken";

        private const string AuthorizationStart = "Bearer ";

        private readonly RequestDelegate _next;

        public CookieAuthMiddleware(RequestDelegate next)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(CookieAuthKey, out var token)
                && !context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.Headers.Add("Authorization", AuthorizationStart + token);
            }

            await this._next(context);
        }
    }
}
