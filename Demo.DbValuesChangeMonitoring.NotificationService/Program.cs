using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");

//builder.UseWolverine(opts =>
//	{
//		opts.UseRabbitMq();
//		opts.PublishAllMessages()
//			.ToRabbitQueue("test");
//	}
//);

var connectionString =new Uri( builder.Configuration.GetConnectionString("rmq"));

ConnectionFactory factory = new ConnectionFactory()
{
	Uri = connectionString,
	AutomaticRecoveryEnabled = true
};
var connection = factory.CreateConnection();

var channel = connection.CreateModel();
channel.QueueDeclare("test", true,false,false);

channel.Close();
channel.Dispose();
connection.Close();
connection.Dispose();

builder.AddRabbitMQClient("rmq");

var host = builder.Build();
//await host.StartAsync();
host.Run();
