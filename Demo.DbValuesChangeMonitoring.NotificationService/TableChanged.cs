
namespace Demo.DbValuesChangeMonitoring.NotificationService
{
	public record TableChanged(string TableName)
	{
		public DateTime MessageCreated { get => DateTime.UtcNow; }
	}
}
