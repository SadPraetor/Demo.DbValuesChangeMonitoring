using Microsoft.Data.SqlClient;
using System.Text;
using Wolverine;

namespace Demo.DbValuesChangeMonitoring.NotificationService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly QueueConsumer _queueConsumer;
    public Worker(IServiceScopeFactory serviceScopeFactory, 
        QueueConsumer queueConsumer,
        IHostApplicationLifetime hostApplicationLifetime,
        IConfiguration configuration,
        ILogger<Worker> logger)
    {
		_serviceScopeFactory = serviceScopeFactory;
		_queueConsumer = queueConsumer;
		_hostApplicationLifetime = hostApplicationLifetime;
		_configuration = configuration;
		_logger = logger;
    }

    private static string sqlQuery = """
        WAITFOR (
            RECEIVE TOP(1)
                message_body
            FROM ValuesChangeEventQueue
        ), TIMEOUT 90000;
        """;
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

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var message = await _queueConsumer.ReadQueueAsync(stoppingToken); 

            try
            {
                if (message.HasValue)
                {
                    await messageBus.PublishAsync(new TableChanged("configuration.ConfigurationValues"));                    
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to publish message");
                throw;
            }            
        }
    }
}
