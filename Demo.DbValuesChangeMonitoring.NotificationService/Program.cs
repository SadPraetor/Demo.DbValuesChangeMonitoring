using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");

var rmqConnectionString =new Uri( builder.Configuration.GetConnectionString("rmq")!);
var sqlConnectionString = builder.Configuration.GetConnectionString("ValuesChangedMonitoring");

builder.UseWolverine(opts =>
	{
		opts.UseRabbitMq(rmqConnectionString)
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
			.UseDurableOutbox();
	}
);


var host = builder.Build();
host.Run();
