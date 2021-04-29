using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using Microsoft.ApplicationInsights.Channel;
using Project.ServiceFabric.Logging.Abstractions.Constants;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace Project.ServiceFabric.Logging.ApplicationInsights
{
    internal class ApplicationInsightConverter : ITelemetryConverter
    {
        private readonly ServiceContext _context;

        public ApplicationInsightConverter(ServiceContext context)
        {
            _context = context;
        }

        public IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            yield return LogEventToTelemetryConverter(logEvent);
        }

        private ITelemetry LogEventToTelemetryConverter(LogEvent logEvent)
        {
            int serviceFabricEvent = ServiceFabricEvent.Undefined;
            if (logEvent.Properties.TryGetValue(SharedProperties.EventId, out LogEventPropertyValue eventId))
            {
                Int32.TryParse(((StructureValue)eventId).Properties[0].Value.ToString(), out serviceFabricEvent);
            }

            ITelemetry telemetry = serviceFabricEvent switch
            {
                ServiceFabricEvent.Exception => new ExceptionTelemetryFactory(logEvent).CreateModel(),
                ServiceFabricEvent.ApiRequest => new RequestTelemetryFactory(logEvent).CreateModel(),
                ServiceFabricEvent.Metric => new MetricTelemetryFactory(logEvent).CreateModel(),
                ServiceFabricEvent.ServiceRequest => new DependencyTelemetryFactory(logEvent).CreateModel(),
                ServiceFabricEvent.Dependency => new DependencyTelemetryFactory(logEvent).CreateModel(),
                _ => new TraceTelemetryFactory(logEvent).CreateModel()
            };
            SetContextProperties(telemetry, logEvent);

            return telemetry;
        }
        private void SetContextProperties(ITelemetry telemetry, LogEvent logEvent)
        {
            telemetry.Context.Cloud.RoleName = FabricEnvironmentVariable.ServicePackageName;
            telemetry.Context.Cloud.RoleInstance = FabricEnvironmentVariable.ServicePackageActivationId ?? FabricEnvironmentVariable.ServicePackageInstanceId;
            telemetry.Context.Component.Version = _context.CodePackageActivationContext.CodePackageVersion;

            if (!telemetry.Context.GlobalProperties.ContainsKey(ServiceContextProperties.NodeName))
            {
                if (!string.IsNullOrEmpty(FabricEnvironmentVariable.NodeName))
                {
                    telemetry.Context.GlobalProperties.Add(ServiceContextProperties.NodeName, FabricEnvironmentVariable.NodeName);
                }
            }

#if Debug
            telemetry.Context.Operation.SyntheticSource = "DebugSession";
#else
            if (Debugger.IsAttached)
            {
                telemetry.Context.Operation.SyntheticSource = "DebuggerAttached";
            }
#endif

            if (logEvent.Properties.TryGetValue(SharedProperties.TraceId, out LogEventPropertyValue value))
            {
                var id = ((ScalarValue)value).Value.ToString();
                telemetry.Context.Operation.ParentId = id;
                telemetry.Context.Operation.Id = id;
            }
        }
    }
}
