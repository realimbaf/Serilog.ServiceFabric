using System;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Serilog.Events;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Mappings.Telemetry
{
    internal class RequestTelemetryFactory : TelemetryFactoryBase
    {
        public RequestTelemetryFactory(LogEvent logEvent) : base(logEvent)
        {
        }
        internal override ITelemetry CreateModel()
        {
            var requestTelemetry = new RequestTelemetry
            {
                ResponseCode = TryGetStringValue(ApiRequestProperties.StatusCode),
                Url = new Uri($"{TryGetStringValue(ApiRequestProperties.Scheme)}://{TryGetStringValue(ApiRequestProperties.Host)}{TryGetStringValue(ApiRequestProperties.Path)}"),
                Name = $"{TryGetStringValue(ApiRequestProperties.Method)} {TryGetStringValue(ApiRequestProperties.Path)}",
                Timestamp = DateTime.Parse(TryGetStringValue(ApiRequestProperties.StartTime)),
                Duration = TimeSpan.FromMilliseconds(Double.Parse(TryGetStringValue(ApiRequestProperties.DurationInMs))),
                Properties =
                {
                    { ApiRequestProperties.Method, TryGetStringValue(ApiRequestProperties.Method) },
                    { ApiRequestProperties.Query, TryGetStringValue(ApiRequestProperties.Query) },
                    { ApiRequestProperties.RequestHeaders, TryGetStringValue(ApiRequestProperties.RequestHeaders) },
                    { ApiRequestProperties.RequestBody,  TryGetStringValue(ApiRequestProperties.RequestBody) },
                    { ApiRequestProperties.ResponseHeaders,  TryGetStringValue(ApiRequestProperties.ResponseHeaders)},
                    { ApiRequestProperties.ResponseBody,  TryGetStringValue(ApiRequestProperties.ResponseBody)}
                }
            };
            requestTelemetry.Context.Operation.Name = requestTelemetry.Name;
            requestTelemetry.Id = TryGetStringValue(SharedProperties.TraceId);

            AddLogEventProperties(requestTelemetry, new[] 
            {
                ApiRequestProperties.DurationInMs,
                ApiRequestProperties.Host,
                ApiRequestProperties.Method,
                ApiRequestProperties.Path,
                ApiRequestProperties.Scheme,
                ApiRequestProperties.StartTime,
                ApiRequestProperties.StatusCode
            });
            return requestTelemetry;
        }

       
    }
}
