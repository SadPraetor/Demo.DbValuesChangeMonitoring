using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.RabbitMQ;


namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public class DbOptionsProvider : ConfigurationProvider
	{
		private readonly Uri _rmqConnectionString;
		private readonly string _sqlConnectionString;
		private readonly IHost _host;

		public DbOptionsProvider(Uri rmqConnectionString, string sqlConnectionString)
		{
			_rmqConnectionString = rmqConnectionString;
			_sqlConnectionString = sqlConnectionString;
			var builder = Host.CreateDefaultBuilder();
			builder.ConfigureServices(sp => sp.AddSingleton<DbOptionsProvider>(this));
			builder.UseWolverine(opt =>
			{
				opt.UseRabbitMq(rmqConnectionString)
					.DisableDeadLetterQueueing();

				opt.ListenToRabbitQueue("db.value_change.configuration.ConfigurationValues");

			});
			_host = builder.Start();
		}

		public void Reload(object sender)
		{
			Load();
			OnReload();
		}

		public override void Load()
		{
			using (var connection = new SqlConnection(_sqlConnectionString))
			{
				connection.Open();

				using (var command = new SqlCommand("SELECT * FROM configuration.ConfigurationValues", connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var key = reader["key"].ToString();
							var value = reader["value"].ToString() ;

							if(!string.IsNullOrEmpty(key))
							{
								Data[key] = value;
							}
						}
					}
				}
			}
		}

		public void Dispose()
		{
			_host?.StopAsync();
			_host?.Dispose();
		}
	}
}
