using Demo.DbValuesChangeMonitoring.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.DbValuesChangeMonitoring.MigrationService;

public class Worker : BackgroundService
{
	private readonly IServiceProvider _provider;
	private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider provider, ILogger<Worker> logger)
    {
		_provider = provider;
		_logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConfigurationContext>();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")=="Development")
        {
            await context.Database.MigrateAsync();
            //_logger.LogInformation("Migration applied");
        }
        
       
    }
}
