using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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

			return builder;
		}
	}
}
