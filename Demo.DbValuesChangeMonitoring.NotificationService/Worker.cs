using Wolverine;

namespace Demo.DbValuesChangeMonitoring.NotificationService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    public Worker(IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger)
    {
		_serviceScopeFactory = serviceScopeFactory;
		_hostApplicationLifetime = hostApplicationLifetime;
		_logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, _hostApplicationLifetime.ApplicationStarted);
        }
        catch
        { 
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await messageBus.PublishAsync(new TableChanged("configuration.ConfigurationValues"));                   
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Failed to publish message");
                    throw;
                }
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}
