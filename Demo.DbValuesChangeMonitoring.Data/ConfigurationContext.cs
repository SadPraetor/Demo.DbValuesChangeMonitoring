using Microsoft.EntityFrameworkCore;

namespace Demo.DbValuesChangeMonitoring.Data
{
	public class ConfigurationContext : DbContext
	{
		public ConfigurationContext(DbContextOptions<ConfigurationContext> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.HasDefaultSchema("configuration");
			builder.ApplyConfigurationsFromAssembly(typeof(ConfigurationContext).Assembly);
		}

		public DbSet<ConfigurationValue> ConfigurationValues { get; set; }
	}
}
