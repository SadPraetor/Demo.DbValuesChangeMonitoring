var builder = DistributedApplication.CreateBuilder(args);


builder.AddSqlServer("sql")
	.WithImageTag("2022-CU16-ubuntu-22.04")
	.WithContainerName("monitor_change")
	.WithLifetime(ContainerLifetime.Session);


builder.Build().Run();
