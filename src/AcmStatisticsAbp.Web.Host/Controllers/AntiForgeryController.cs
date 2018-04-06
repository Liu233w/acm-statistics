// <copyright file="AntiForgeryController.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Web.Host.Controllers
{
    using AcmStatisticsAbp.Controllers;
    using Microsoft.AspNetCore.Antiforgery;

    public class AntiForgeryController : AcmStatisticsAbpControllerBase
    {
        private readonly IAntiforgery antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }

        public void GetToken()
        {
            this.antiforgery.SetCookieTokenAndHeader(this.HttpContext);
        }
    }
}
