# Serilog.ServiceFabric
Write Service Fabric Serilog events to Application Insights


To configure ILoggerFactory:
```csharp
ILoggerFactory loggerFactory = new LoggerFactoryBuilder(context).CreateLoggerFactory("ApplicationInsightsKey");
loggerFactory.CreateLogger<Service>()

