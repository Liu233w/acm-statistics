using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;

namespace OHunt.Web.Schedule
{
    public class ScheduleCrawlerService : IHostedService, IDisposable
    {
        private readonly ILogger<ScheduleCrawlerService> _logger;
        private Timer? _timer;
        private readonly SubmissionCrawlerCoordinator _coordinator;

        private readonly ZojSubmissionCrawler _zojSubmissionCrawler
            = new ZojSubmissionCrawler();

        private volatile int _isRunning = 0;

        public ScheduleCrawlerService(
            ILogger<ScheduleCrawlerService> logger,
            SubmissionCrawlerCoordinator coordinator)
        {
            _logger = logger;
            _coordinator = coordinator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(0.5));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void DoWork(object? state)
        {
            _logger.LogTrace("Starting crawler schedule");

            if (Interlocked.Exchange(ref _isRunning, 1) == 1)
            {
                _logger.LogInformation("Previous crawler is not finished yet");
                return;
            }

            try
            {
                Task.WaitAll(
                    _coordinator.WorkAsync(_zojSubmissionCrawler));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception when running worker");
            }

            _logger.LogTrace("All crawler finished");
            _isRunning = 0;
        }
    }
}
