using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");

var host = builder.Build();
host.Run();
