using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings
{
    internal abstract class TelemetryFactoryBase
    {
        private readonly LogEvent _logEvent;

        protected TelemetryFactoryBase(LogEvent logEvent)
        {
            _logEvent = logEvent;
        }

        internal abstract ITelemetry CreateModel();

        protected string TryGetStringValue(string propertyName)
        {
            if (!_logEvent.Properties.TryGetValue(propertyName, out LogEventPropertyValue value))
                throw new ArgumentException($"LogEvent does not contain required property {propertyName} for EventId {_logEvent.Properties[SharedProperties.EventId]}", propertyName);

            var logValue = (ScalarValue)value;

            if (logValue?.Value == null)
                return null;

            return logValue.Value.ToString();
        }

        protected void AddLogEventProperties(ISupportProperties telemetry, IEnumerable<string> excludePropertyKeys = null)
        {
            var excludedPropertyKeys = new List<string>
            {
                ServiceContextProperties.NodeName,
                ServiceContextProperties.ServicePackageVersion,
                ServiceContextProperties.ApplicationTypeName
            };

            if (excludePropertyKeys != null)
                excludedPropertyKeys.AddRange(excludePropertyKeys);

            foreach (var property in _logEvent
                .Properties
                .Where(property => property.Value != null &&
                                   !excludedPropertyKeys.Contains(property.Key) &&
                                   !telemetry.Properties.ContainsKey(property.Key)))
            {
                ApplicationInsightsPropertyFormatter.WriteValue(property.Key, property.Value, telemetry.Properties);
            }
        }
    }
}
