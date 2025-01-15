using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public sealed class QueueConsumer : IDisposable
	{
		private readonly ILogger<QueueConsumer> _logger;
		private readonly string sqlConnectionString;
		private static readonly string _sqlQuery = """
        WAITFOR (
            RECEIVE TOP(1)
                message_body
            FROM ValuesChangeEventQueue
        ), TIMEOUT 90000;
        """;
		private readonly Lazy<SqlConnection> _sqlConnection;

		private readonly Lazy<SqlCommand> _sqlCommand;
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

		public async Task ProcessQueueAsync(Func<string, Task> nextStep, CancellationToken cancellationToken = default)
		{
			if (_sqlConnection.Value.State is not System.Data.ConnectionState.Open)
			{
				await _sqlConnection.Value.OpenAsync();
			}

			while (!cancellationToken.IsCancellationRequested)
			{
				using var transaction = await _sqlConnection.Value.BeginTransactionAsync();
				try
				{
					var casted = transaction as SqlTransaction;
					_sqlCommand.Value.Transaction = casted;

					byte[] messageBody = default!;
					using (var reader = await _sqlCommand.Value.ExecuteReaderAsync(cancellationToken))
					{
						if (await reader.ReadAsync(cancellationToken))
						{
							messageBody = (byte[])reader["message_body"];
						}
					}

					Debug.Assert(messageBody is not null);
					var message = Encoding.Unicode.GetString(messageBody);

					if (!string.IsNullOrEmpty(message))
					{
						await nextStep(message);							
					}						
					await transaction.CommitAsync();	
				}
				catch (SqlException ex) when (ex.Number == -2)
				{
					await transaction.RollbackAsync();
				}
				catch(TaskCanceledException)
				{
					await transaction.RollbackAsync();					
					return;
				}
				catch (Exception exception)
				{
					await transaction.RollbackAsync();
					_logger.LogError(exception, "Error while reading queue");
					throw;
				}
				finally
				{
					_sqlCommand.Value.Transaction = null;
				}
			}			
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
