using System.Fabric;
using System.Globalization;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Project.ServiceFabric.Logging.Abstractions.Constants;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Project.ServiceFabric.Logging.ApplicationInsights;
using Project.ServiceFabric.Logging.ApplicationInsights.Providers;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using ServiceFabric.Remoting.CustomHeaders;

namespace Project.ServiceFabric.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILoggerFactoryBuilder"/> 
    /// </summary>
    public class LoggerFactoryBuilder : ILoggerFactoryBuilder
    {
        private readonly ServiceContext _context;

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="context">The <see cref="ServiceContext"/> of the service or actor to monitor</param>
        public LoggerFactoryBuilder(ServiceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates an instance of <see cref="ILoggerFactory"/> that provides logging to application insights using Serilog
        /// </summary>
        /// <param name="sectionName">Section name from service fabric config</param>
        /// <param name="parameterName">Parameter name from service fabric config</param>
        /// <returns>An instance of <see cref="LoggerFactory"/></returns>
        public ILoggerFactory CreateLoggerFactory(string sectionName, string parameterName)
        {
            string applicationInsightsKey = FabricRuntime.GetActivationContext()
                .GetConfigurationPackageObject("Config")
                .Settings
                .Sections[sectionName]
                .Parameters[parameterName]
                .Value;
            return CreateLoggerFactory(applicationInsightsKey);
        }
        /// <summary>
        /// Creates an instance of <see cref="ILoggerFactory"/> that provides logging to application insights using Serilog
        /// </summary>
        /// <param name="aiKey">The Application Insights key used for logging</param>
        /// <returns>An instance of <see cref="LoggerFactory"/></returns>
        public ILoggerFactory CreateLoggerFactory(string aiKey)
        {
            var configuration = new TelemetryConfiguration
            {
                InstrumentationKey = aiKey
            };
            
            configuration.TelemetryInitializers
                .Add(new OperationContextTelemetryInitializer(
                    () => RemotingContext.GetData(HeaderIdentifiers.TraceId)?.ToString()));
            configuration.EnableLiveStream();
            
            var loggerFactory = new LoggerFactory();
            Logger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .ApplicationInsights(configuration, new ApplicationInsightConverter(_context))
                .CreateLogger();

            InitContextProperties();

            loggerFactory.AddSerilog(logger, true);

            return loggerFactory;
        }

        private void InitContextProperties()
        {
            LogContext.PushProperty(ServiceContextProperties.ServiceTypeName, _context.ServiceTypeName);
            LogContext.PushProperty(ServiceContextProperties.ServiceName, _context.ServiceName);
            LogContext.PushProperty(ServiceContextProperties.PartitionId, _context.PartitionId);
            LogContext.PushProperty(ServiceContextProperties.NodeName, _context.NodeContext.NodeName);
            LogContext.PushProperty(ServiceContextProperties.ApplicationName, _context.CodePackageActivationContext.ApplicationName);
            LogContext.PushProperty(ServiceContextProperties.ApplicationTypeName, _context.CodePackageActivationContext.ApplicationTypeName);
            LogContext.PushProperty(ServiceContextProperties.ServicePackageVersion, _context.CodePackageActivationContext.CodePackageVersion);

            if (_context is StatelessServiceContext)
            {
                LogContext.PushProperty(ServiceContextProperties.InstanceId, _context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
            }
            else if (_context is StatefulServiceContext)
            {
                LogContext.PushProperty(ServiceContextProperties.ReplicaId, _context.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}


