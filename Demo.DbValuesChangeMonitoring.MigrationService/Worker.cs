using Demo.DbValuesChangeMonitoring.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.DbValuesChangeMonitoring.MigrationService;

public class Worker : BackgroundService
{
	private readonly IServiceProvider _provider;
	private  IHostApplicationLifetime _hostApplication;
	private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider provider, 
        IHostApplicationLifetime hostApplication,
        ILogger<Worker> logger)
    {
		_provider = provider;
		_hostApplication = hostApplication;
		_logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Migration Worker running at: {time}", DateTimeOffset.Now);
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ConfigurationContext>();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")=="Development")
        {
            await context.Database.MigrateAsync();
            _logger.LogInformation("Migration applied");
        }

        await SeedApplication(context);

        _hostApplication.StopApplication();
    }

    private async Task SeedApplication(ConfigurationContext context)
    {
        if ((await context.ConfigurationValues.AnyAsync()))
        {
            return;
        }

        ConfigurationValue[] data = [
            new ConfigurationValue("test1","decimal","12.9"),
            new ConfigurationValue("test2","string","mytestvalue"),
			new ConfigurationValue("test3","int","3")
		];

        context.AddRange(data);

        await context.SaveChangesAsync();
        _logger.LogInformation("Application data seeded");
    }
}
