using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Extensions;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry
{
    internal class ExceptionTelemetryFactory : TelemetryFactoryBase
    {
        private readonly LogEvent _logEvent;

        public ExceptionTelemetryFactory(LogEvent logEvent) : base(logEvent)
        {
            _logEvent = logEvent;
        }

        internal override ITelemetry CreateModel()
        {
            var exceptionTelemetry = new ExceptionTelemetry(_logEvent.Exception)
            {
                SeverityLevel = _logEvent.Level.ToSeverityLevel(),
                Timestamp = _logEvent.Timestamp,
                Message = _logEvent.RenderMessage()
            };
            return exceptionTelemetry;
        }
    }
}
