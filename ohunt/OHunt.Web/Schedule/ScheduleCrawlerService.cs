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

        private readonly ZojSubmissionCrawler _zojSubmissionCrawler;

        private volatile int _isRunning = 0;
        private volatile Task[]? _runningTask = null;

        private volatile CancellationTokenSource _tokenSource = new CancellationTokenSource();

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(0.5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);
            _tokenSource.Cancel();

            return _runningTask != null
                ? Task.WhenAll(_runningTask)
                : Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void DoWork(object? state)
        {
            _logger.LogTrace("Starting crawler schedule");

            if (Interlocked.Exchange(ref _isRunning, 1) == 1)
            {
                _logger.LogInformation("Previous crawler is not finished yet");
                return;
            }

            _runningTask = new[]
            {
                _coordinator.WorkAsync(_zojSubmissionCrawler, _tokenSource.Token),
            };
            try
            {
                Task.WaitAll(_runningTask);
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
