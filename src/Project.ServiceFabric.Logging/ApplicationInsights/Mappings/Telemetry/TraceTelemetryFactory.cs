using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Extensions;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry
{
    internal class TraceTelemetryFactory : TelemetryFactoryBase
    {
        private readonly LogEvent _logEvent;

        public TraceTelemetryFactory(LogEvent logEvent) : base(logEvent)
        {
            _logEvent = logEvent;
        }

        internal override ITelemetry CreateModel()
        {
            var traceTelemetry = new TraceTelemetry(_logEvent.RenderMessage())
            {
                SeverityLevel = _logEvent.Level.ToSeverityLevel(),
                Timestamp = _logEvent.Timestamp
            };

            AddLogEventProperties(traceTelemetry);

            return traceTelemetry;
        }
    }
}
