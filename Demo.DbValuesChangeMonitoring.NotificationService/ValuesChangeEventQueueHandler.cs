using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolverine;

namespace Demo.DbValuesChangeMonitoring.NotificationService
{
	public record TableChange(string Body);
	public class ValuesChangeEventQueueHandler
	{
		public async Task Handle(TableChange data)
		{
			
			Console.WriteLine(data.Body);
		}
	}
}
