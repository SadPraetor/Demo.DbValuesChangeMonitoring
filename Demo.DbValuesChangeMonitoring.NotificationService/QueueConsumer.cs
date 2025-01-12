using Microsoft.Data.SqlClient;
using Optional;
using System.Text;

namespace Demo.DbValuesChangeMonitoring.NotificationService
{
	public class QueueConsumer : IDisposable
	{
		private readonly ILogger<QueueConsumer> _logger;
		private readonly string sqlConnectionString;
		private static string _sqlQuery = """
        WAITFOR (
            RECEIVE TOP(1)
                message_body
            FROM ValuesChangeEventQueue
        ), TIMEOUT 90000;
        """;
		private Lazy<SqlConnection> _sqlConnection;

		private Lazy<SqlCommand> _sqlCommand;
		public QueueConsumer(IConfiguration configuration, ILogger<QueueConsumer> logger)
		{
			sqlConnectionString = configuration.GetConnectionString("ValuesChangedMonitoring")!;
			_sqlConnection = new Lazy<SqlConnection>(() => new SqlConnection(sqlConnectionString));
			_sqlCommand = new Lazy<SqlCommand>(() =>
			{
				var command = new SqlCommand(_sqlQuery, _sqlConnection.Value);
				return command;
			});
			_logger = logger;
		}

		public async Task<Option<string>> ReadQueueAsync(CancellationToken cancellationToken = default)
		{
			if (_sqlConnection.Value.State is not System.Data.ConnectionState.Open)
			{
				await _sqlConnection.Value.OpenAsync();
			}

			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					using var reader = await _sqlCommand.Value.ExecuteReaderAsync(cancellationToken);
					if (await reader.ReadAsync(cancellationToken))
					{
						var messageBody = (byte[])reader["message_body"];
						var message = Encoding.Unicode.GetString(messageBody);

						if(!string.IsNullOrEmpty(message))
						{
							return Option.Some(message);
						}
					}
				}
				catch (SqlException ex) when (ex.Number == -2)
				{

				}
				catch(TaskCanceledException)
				{
					return Option.None<string>();
				}
				catch (Exception exception)
				{
					_logger.LogError(exception, "Error while reading queue");
					throw;
				}
			}

			return Option.None<string>();
		}

		public void Dispose()
		{
			_logger.LogInformation("Dispose called");
			if (_sqlCommand.IsValueCreated)
			{
				_sqlCommand.Value.Dispose();
			}


			if (!_sqlConnection.IsValueCreated)
			{
				return;
			} 

			if(_sqlConnection.Value.State is System.Data.ConnectionState.Open)
			{
				_sqlConnection.Value.Close();
			}

			_sqlConnection.Value.Dispose();
		}
	}
}
