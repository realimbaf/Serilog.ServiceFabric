# Serilog.ServiceFabric
Write Service Fabric Serilog events to Application Insights


To configure the sink in C# code, call WriteTo.File() during logger configuration:

var log = new LoggerConfiguration()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
