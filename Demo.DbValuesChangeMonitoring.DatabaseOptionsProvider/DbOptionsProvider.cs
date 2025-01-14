using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider
{
	public class DbOptionsProvider : ConfigurationProvider
	{
		private readonly string _connectionString;
		

		public DbOptionsProvider(string connectionString)
		{
			_connectionString = connectionString;
			
		}

		private void Reload(object sender)
		{
			
			OnReload();
		}

		public override void Load()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();

				using (var command = new SqlCommand("SELECT * FROM configuration.ConfigurationValues", connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var key = reader["key"].ToString();
							var value = reader["value"].ToString() ;

							if(!string.IsNullOrEmpty(key))
							{
								Data[key] = value;
							}
						}
					}
				}
			}
		}

		public void Dispose()
		{
			
		}
	}
}
