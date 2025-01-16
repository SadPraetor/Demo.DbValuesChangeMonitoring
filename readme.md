# Demo.DbValuesMonitoring

#### Aims
- explore `Aspire`
- explore `WolverineFX` nuget library
- demonstrate `OptionsProvider` that reads configuration values from database, and is able to monitor for change

### Notification Service
Background service is continously running and checking in MS SQL Service Broker Queue. Picked queue message is then 
published into RabbitMQ. For publishing `Wolverine` library is used.  
`Wolverine` is not able to monitor queue. For that reason simple `SqlCommand` is used. Command is run for 90s repeatedly.


### DbValueChangeMonitoring
Core of the monitoring is in `DbOptionsProvider.cs`. `Wolverine` is used, for this reason a separate 
host application is started. This application is continously monitoring RabbitMQ queue, 
and on signal received reloads options. 
