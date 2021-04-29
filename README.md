# Serilog.ServiceFabric
Write Serilog events in Service Fabric to Application Insights by overriding the standard .net core IloggerFactory


To configure ILoggerFactory:
```csharp
ILoggerFactory loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory("ApplicationInsightsKey");
loggerFactory.CreateLogger<Service>();
```
Then you can write logs using ILogger interface:

```csharp
_logger.LogInformation("{0}, operation key: {1}, message: {2}", eventName, key, message);
```
