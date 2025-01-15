using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.DatabaseOptionsProvider;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddChangeMonitoring();

var host = builder.Build();
host.Run();
