using Demo.DbValuesChangeMonitoring.NotificationService;

namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	
	public class RabbitMQStringHandler
	{
		public void Handle (TableChanged message)
		{
			Console.WriteLine(message);
		}
	}
}
