using Microsoft.Extensions.Configuration;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public class DbOptionsSource : IConfigurationSource
	{
		private readonly Uri _rmqConnectionString;
		private readonly string _sqlConnectionString;

		public DbOptionsSource(Uri rmqConnection,string sqlConnectionString)
		{
			_rmqConnectionString = rmqConnection;
			_sqlConnectionString = sqlConnectionString;
		}

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new DbOptionsProvider(_rmqConnectionString,_sqlConnectionString);
		}
	}
}
