using Wolverine;

namespace Demo.DbValuesChangeMonitoring.NotificationService;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    
    private readonly QueueConsumer _queueConsumer;
    public Worker(IServiceScopeFactory serviceScopeFactory, 
        QueueConsumer queueConsumer,
        IHostApplicationLifetime hostApplicationLifetime,       
        ILogger<Worker> logger)
    {
		_serviceScopeFactory = serviceScopeFactory;
		_queueConsumer = queueConsumer;
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

        using var scope = _serviceScopeFactory.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        Func<string, Task> nextStep = async (string message) =>
        {
            try
            {               
                if (!string.IsNullOrEmpty(message))
                {
                    await messageBus.PublishAsync("configuration.ConfigurationValues");
                    //await messageBus.PublishAsync(new TableChanged("configuration.ConfigurationValues"));
                    _logger.LogInformation("Message published {message}", message);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to publish message");
                throw;
            }
        };


		while (!stoppingToken.IsCancellationRequested)
        {            
            await _queueConsumer.ProcessQueueAsync(
                nextStep: nextStep,
                stoppingToken); 
        }
    }
}
