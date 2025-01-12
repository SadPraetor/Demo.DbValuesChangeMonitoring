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
    public Worker(IServiceScopeFactory serviceScopeFactory, 
        IHostApplicationLifetime hostApplicationLifetime,
        IConfiguration configuration,
        ILogger<Worker> logger)
    {
		_serviceScopeFactory = serviceScopeFactory;
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

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            using var sqlConnection = new SqlConnection(_configuration.GetConnectionString("ValuesChangedMonitoring"));
            await sqlConnection.OpenAsync();
            using var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.CommandTimeout = 95;

            string? message = null;
            try
            {
                using var reader = await sqlCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var messageBody = (byte[])reader["message_body"];
                    message = Encoding.Unicode.GetString(messageBody);

                    _logger.LogInformation("Retrieved message {message}", message);

                }
            }
            catch (SqlException ex) when (ex.Number == -2)
            {

            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Error in reading from db queue");
                throw;
            }
            finally
            {
                if (sqlConnection.State is System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }               
            }


            try
            {
                if (!string.IsNullOrEmpty(message))
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
