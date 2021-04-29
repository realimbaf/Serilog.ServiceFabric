using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry
{
    internal class MetricTelemetryFactory : TelemetryFactoryBase
    {
        private readonly LogEvent _logEvent;

        public MetricTelemetryFactory(LogEvent logEvent) : base(logEvent)
        {
            _logEvent = logEvent;
        }

        internal override ITelemetry CreateModel()
        {
            var metricTelemetry = new MetricTelemetry
            {
                Name = TryGetStringValue(MetricProperties.Name),
                Sum = double.Parse(TryGetStringValue(MetricProperties.Value)),
                Timestamp = _logEvent.Timestamp
            };

            if (_logEvent.Properties.TryGetValue(MetricProperties.MinValue, out LogEventPropertyValue min))
                metricTelemetry.Min = double.Parse(min.ToString());

            if (_logEvent.Properties.TryGetValue(MetricProperties.MaxValue, out LogEventPropertyValue max))
                metricTelemetry.Max = double.Parse(max.ToString());

            AddLogEventProperties(metricTelemetry, typeof(MetricProperties).GetFields().Select(f => f.GetRawConstantValue().ToString()));

            return metricTelemetry;
        }
    }
}
