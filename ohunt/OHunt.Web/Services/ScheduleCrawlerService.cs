using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHunt.Web.Crawlers;
using OHunt.Web.Dataflow;

namespace OHunt.Web.Services
{
    public class ScheduleCrawlerService : IHostedService, IDisposable
    {
        private readonly ILogger<ScheduleCrawlerService> _logger;
        private Timer _timer = null!;
        private readonly SubmissionCrawlerCoordinator _coordinator;

        private readonly ZojSubmissionCrawler _zojSubmissionCrawler;

        public ScheduleCrawlerService(
            ILogger<ScheduleCrawlerService> logger,
            SubmissionCrawlerCoordinator coordinator,
            ZojSubmissionCrawler zojSubmissionCrawler)
        {
            _logger = logger;
            _coordinator = coordinator;
            _zojSubmissionCrawler = zojSubmissionCrawler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _coordinator.Initialize(new ISubmissionCrawler[]
            {
                // _zojSubmissionCrawler,
            });
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(0.5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            return _coordinator.Cancel();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object? state)
        {
            _logger.LogTrace("Starting all crawlers");
            _coordinator.StartAllCrawlers().Wait();
        }
    }
}
