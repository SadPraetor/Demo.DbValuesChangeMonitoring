using Demo.DbValuesChangeMonitoring.Data;
using Demo.DbValuesChangeMonitoring.NotificationService;
using Wolverine;
using Wolverine.RabbitMQ;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<ConfigurationContext>("ValuesChangedMonitoring");

var connectionString =new Uri( builder.Configuration.GetConnectionString("rmq")!);
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
			.UseDurableOutbox();		
	}
);


var host = builder.Build();
host.Run();
