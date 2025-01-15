using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
			

			return builder;
		}

		public static IHostApplicationBuilder AddChangeMonitoring(this IHostApplicationBuilder builder)
		{
			builder.Services.AddHostedService<Worker>();
			builder.Services.AddSingleton<QueueConsumer>();
			var rmqConnectionString = new Uri(builder.Configuration.GetConnectionString("rmq")!);
			builder.UseWolverine(opts =>
			{
				opts.UseRabbitMq(rmqConnectionString)
				.DeclareQueue("db.value_change.configuration.ConfigurationValues", opt =>
				{
					opt.AutoDelete = false;
					opt.IsDurable = true;
					opt.IsExclusive = false;

				})
				.DisableDeadLetterQueueing()
				.AutoProvision();

				opts.PublishAllMessages()
					.ToRabbitQueue("db.value_change.configuration.ConfigurationValues")
					.UseDurableOutbox();
				opts.Durability.Mode = DurabilityMode.Solo;
			}
);

			return builder;
		}
	}
}
