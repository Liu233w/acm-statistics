// <copyright file="HomeController.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Web.Host.Controllers
{
    using System.Threading.Tasks;
    using Abp;
    using Abp.Extensions;
    using Abp.Notifications;
    using Abp.Timing;
    using AcmStatisticsAbp.Controllers;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : AcmStatisticsAbpControllerBase
    {
        private readonly INotificationPublisher notificationPublisher;

        public HomeController(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;
        }

        public IActionResult Index()
        {
            return this.Redirect("/swagger");
        }
    }
}
