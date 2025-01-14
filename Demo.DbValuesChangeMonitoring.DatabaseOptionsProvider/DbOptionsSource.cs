using Microsoft.Extensions.Configuration;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public class DbOptionsSource : IConfigurationSource
	{
		private readonly string _connectionString;

		public DbOptionsSource(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new DbOptionsProvider(_connectionString);
		}
	}
}
