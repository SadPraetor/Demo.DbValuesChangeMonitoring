
using RabbitMQ.Client;
using System.Text;
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
        { }

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();
            //var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await messageBus.PublishAsync(new TableChanged("configuration.ConfigurationValues"));
                    //var channel = connection.CreateModel();
                    
                    //channel.BasicPublish("", "test", null, Encoding.UTF8.GetBytes("configuration.ConfigurationValues"));
                    //channel.Close();
                    //channel.Dispose();
                    
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Unknown Error");
                    throw;
                }
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}
