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
			var rmqConnectionUri = new Uri(builder.Configuration.GetConnectionString("rmq")!);
			var connectionString = builder.Configuration.GetConnectionString("ValuesChangedMonitoring");
			if(string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException("Missing connection string");
			}
			
			builder.Configuration.Add(new DbOptionsSource(rmqConnectionUri,connectionString));
			
			//builder.UseWolverine(opt =>
			//{
			//	opt.UseRabbitMq(rmqConnectionUri)
			//		.DisableDeadLetterQueueing();

			//	opt.ListenToRabbitQueue("db.value_change.configuration.ConfigurationValues");
				
			//});

			return builder;
		}
	}
}
