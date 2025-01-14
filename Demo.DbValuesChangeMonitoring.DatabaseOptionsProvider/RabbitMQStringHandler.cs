using Demo.DbValuesChangeMonitoring.NotificationService;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	
	public class RabbitMQStringHandler
	{
		private readonly DbOptionsProvider _provider; 
		public RabbitMQStringHandler(DbOptionsProvider provider)
		{
			_provider = provider;
		}
		public void Handle (string message)
		{
			Console.WriteLine(message);
			_provider.Reload(message);
		}
	}
}
