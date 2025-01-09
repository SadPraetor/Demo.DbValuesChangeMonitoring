using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.DbValuesChangeMonitoring.Data
{
	public class ConfigurationValueConfiguration : IEntityTypeConfiguration<ConfigurationValue>
	{
		public void Configure(EntityTypeBuilder<ConfigurationValue> builder)
		{
			builder.ToTable(nameof(ConfigurationContext.ConfigurationValues));

			builder.HasKey(x => x.Name);

			builder.Property(x => x.Name)
				.HasMaxLength(255);

			builder.Property(x => x.Type)
				.HasMaxLength(50);

			builder.Property(x => x.Value)
				.HasMaxLength(4000);
		}
	}
}
