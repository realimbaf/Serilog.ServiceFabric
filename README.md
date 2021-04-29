# Serilog.ServiceFabric
Write Service Fabric Serilog events to Application Insights


To configure ILoggerFactory:
```csharp
ILoggerFactory loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory("ApplicationInsightsKey");
loggerFactory.CreateLogger<Service>();
```
Then you can write logs using ILogger interface:

```csharp
_logger.LogInformation("{0}, operation key: {1}, message: {2}", eventName, key, message);
```
