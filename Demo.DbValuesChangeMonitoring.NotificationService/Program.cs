using Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddChangeMonitoring();

var host = builder.Build();
host.Run();
