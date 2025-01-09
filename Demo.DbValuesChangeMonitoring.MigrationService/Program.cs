using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.MigrationService;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");



var host = builder.Build();
host.Run();
