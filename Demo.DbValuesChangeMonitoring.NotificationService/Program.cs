using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Wolverine;
using Wolverine.RabbitMQ;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");

var connectionString =new Uri( builder.Configuration.GetConnectionString("rmq"));
builder.UseWolverine(opts =>
	{
		opts.UseRabbitMq(connectionString)
		.DeclareQueue("db.value_change.configuration.ConfigurationValues",opt=>
		{
			opt.AutoDelete = false;
			opt.IsDurable = true;
			opt.IsExclusive = false;
			
		})
		
		.DisableDeadLetterQueueing()
		
		.AutoProvision();
		opts.PublishAllMessages()
			.ToRabbitQueue("db.value_change.configuration.ConfigurationValues")
			.UseDurableOutbox()
			;
		
	}
);


//ConnectionFactory factory = new ConnectionFactory()
//{
//	Uri = connectionString,
//	AutomaticRecoveryEnabled = true
//};
//var connection = factory.CreateConnection();

//var channel = connection.CreateModel();
//channel.QueueDeclare("test", true,false,false);

//channel.Close();
//channel.Dispose();
//connection.Close();
//connection.Dispose();

//builder.AddRabbitMQClient("rmq");

var host = builder.Build();
//await host.StartAsync();
host.Run();
