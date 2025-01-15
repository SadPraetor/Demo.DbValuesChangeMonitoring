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
			new ConfigurationValue("Account:SpendLimit","decimal","30"),
            new ConfigurationValue("Notifications:Email","string","Enabled"),
            new ConfigurationValue("Security:TwoFactorAuth","bool","True"),
            new ConfigurationValue("Display:FontSize","string","Medium"),
            new ConfigurationValue("Display:Theme","string","Dark"),
            new ConfigurationValue("Display:NavigationBar","string","Right"),
            new ConfigurationValue("Privacy:DataSharing","string","ConsentRequired"),
            new ConfigurationValue("Localization:Language","string","English"),
            new ConfigurationValue("Performance:CacheSize","string","256MB"),
            new ConfigurationValue("Accessibility:VoiceOver","bool","On"),
            new ConfigurationValue("Backup:Frequency","string","Weekly"),
            new ConfigurationValue("Network:Timeout","string","60s"),
            new ConfigurationValue("System:UpdateChannel","string","Stable"),
		];

		context.AddRange(data);

        await context.SaveChangesAsync();
        _logger.LogInformation("Application data seeded");
    }
}
