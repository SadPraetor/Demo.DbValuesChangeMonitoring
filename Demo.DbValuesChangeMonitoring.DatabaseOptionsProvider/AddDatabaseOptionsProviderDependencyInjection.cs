using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public static class AddDatabaseOptionsProviderDependencyInjection
	{
		public static IHostApplicationBuilder AddDatabaseOptionsProvider(this IHostApplicationBuilder builder)
		{
			var connectionString = builder.Configuration.GetConnectionString("ValuesChangedMonitoring");
			if(string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("Missing connection string");
			}
			builder.Configuration.Add(new DbOptionsSource(connectionString));
			var rmqConnectionString = new Uri(builder.Configuration.GetConnectionString("rmq")!);
			builder.UseWolverine(opt =>
			{
				opt.UseRabbitMq(rmqConnectionString)
					.DisableDeadLetterQueueing();

				opt.ListenToRabbitQueue("db.value_change.configuration.ConfigurationValues");
				
			});

			return builder;
		}
	}
}
