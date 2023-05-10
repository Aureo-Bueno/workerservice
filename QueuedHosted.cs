
using WorkerService.Service;

namespace WorkerService
{
    public class QueuedHosted : BackgroundService
    {
        private readonly IBackGroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHosted> _logger;

        public QueuedHosted(IBackGroundTaskQueue taskQueue, ILogger<QueuedHosted> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(QueuedHosted)} is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            return ProcessTaskQueueAsync(stoppingToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Func<CancellationToken, ValueTask>? workItem =
                        await _taskQueue.DequeueAsync(stoppingToken);

                    await workItem(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if stoppingToken was signaled
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing task work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(QueuedHosted)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
