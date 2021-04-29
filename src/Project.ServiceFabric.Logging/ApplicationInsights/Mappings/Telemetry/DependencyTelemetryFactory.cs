using System;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry
{
    internal class DependencyTelemetryFactory : TelemetryFactoryBase
    {
        private readonly LogEvent _logEvent;

        public DependencyTelemetryFactory(LogEvent logEvent) : base(logEvent)
        {
            _logEvent = logEvent;
        }

        internal override ITelemetry CreateModel()
        {
            var dependencyTelemetry = new DependencyTelemetry
            {
                Name = TryGetStringValue(DependencyProperties.DependencyTypeName),
                Data = TryGetStringValue(DependencyProperties.Data),
                Type = TryGetStringValue(DependencyProperties.Type)
            };

            if (_logEvent.Properties.TryGetValue(DependencyProperties.DurationInMs,
                out LogEventPropertyValue durationInMs))
            {
                dependencyTelemetry.Duration = TimeSpan.FromMilliseconds(double.Parse(((ScalarValue)durationInMs).Value.ToString()));
            }
            if (_logEvent.Properties.TryGetValue(DependencyProperties.Success, out LogEventPropertyValue success))
            {
                dependencyTelemetry.Success = bool.Parse(((ScalarValue)success).Value.ToString());
            }

            if (_logEvent.Properties.TryGetValue(DependencyProperties.StartTime,
                out LogEventPropertyValue startTime))
            {
                dependencyTelemetry.Timestamp = DateTime.Parse(((ScalarValue)startTime).Value.ToString());
            }

            dependencyTelemetry.Id = dependencyTelemetry.Data;
            dependencyTelemetry.Context.Operation.Name = dependencyTelemetry.Name;

            AddLogEventProperties(dependencyTelemetry, typeof(DependencyProperties).GetFields().Select(f => f.GetRawConstantValue().ToString()));

            return dependencyTelemetry;
        }
    }
}
