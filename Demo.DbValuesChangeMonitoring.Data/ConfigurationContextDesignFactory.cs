using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Demo.DbValuesChangeMonitoring.Data
{
	public class ConfigurationContextDesignFactory : IDesignTimeDbContextFactory<ConfigurationContext>
	{
		public ConfigurationContext CreateDbContext(string[] args)
		{
			var builder = new DbContextOptionsBuilder<ConfigurationContext>();
			builder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Configuration;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
			return new ConfigurationContext(builder.Options);
		}
	}
}
