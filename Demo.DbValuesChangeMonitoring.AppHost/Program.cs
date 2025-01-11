

using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);



var db =builder.AddSqlServer("sql",port:1433)	
	
	.WithImageTag("2022-CU16-ubuntu-22.04")
	.WithContainerName("monitor_change")
	.WithEnvironment("ACCEPT_EULA", "Y")
	.WithEnvironment("TrustServerCertificate","True")
	.WithEnvironment("Encrypt", "True")
	.WithLifetime(ContainerLifetime.Persistent)	
	.AddDatabase("ValuesChangedMonitoring");


builder.AddProject<Projects.Demo_DbValuesChangeMonitoring_MigrationService>("migration")
	.WithEnvironment("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
	.WithReference(db)
	.WaitFor(db);

builder.AddRabbitMQ("rmq")	
	.WithImageTag("4.0.5-alpine")
	.WithManagementPlugin()
	.WithLifetime(ContainerLifetime.Session)	
	.WaitFor(db);

builder.Build().Run();
