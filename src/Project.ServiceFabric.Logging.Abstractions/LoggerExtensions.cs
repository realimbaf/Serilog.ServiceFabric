using System;
using Microsoft.Extensions.Logging;
using Project.ServiceFabric.Logging.Abstractions.Constants;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Project.ServiceFabric.Logging.Abstractions.Models;

namespace Project.ServiceFabric.Logging.Abstractions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Extension for Logging metrics
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void LogMetric(this ILogger logger, string name, double value)
        {
            logger.LogInformation(ServiceFabricEvent.Metric,
                $"Metric {{{MetricProperties.Name}}}, value: {{{MetricProperties.Value}}}",
                name,
                value);
        }

        /// <summary>
        /// Extension for Logging metrics
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public static void LogMetric(this ILogger logger, string name, double value, double max, double min)
        {
            logger.LogInformation(ServiceFabricEvent.Metric,
                $"Metric {{{MetricProperties.Name}}}, value: {{{MetricProperties.Value}}}, min: {{{MetricProperties.MinValue}}}, max: {{{MetricProperties.MaxValue}}}",
                name,
                value,
                min,
                max);
        }

        /// <summary>
        /// Extension for Logging dependencies
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceName"></param>
        /// <param name="data"></param>
        public static void LogDependency(this ILogger logger, string serviceName, string data)
        {
            logger.LogInformation(ServiceFabricEvent.ServiceRequest,
                $"The call to {{{DependencyProperties.Type}}} dependency {{{DependencyProperties.DependencyTypeName}}} data {{{DependencyProperties.Data}}}",
                "ServiceFabric",
                serviceName,
                data);
        }
        /// <summary>
        /// Extension for Logging dependencies
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceName"></param>
        /// <param name="data"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <param name="success"></param>
        public static void LogDependency(this ILogger logger, string serviceName, string data,
            DateTime started, TimeSpan duration, bool success)
        {
            logger.LogInformation(ServiceFabricEvent.ServiceRequest,
                $"The call to {{{DependencyProperties.Type}}} dependency {{{DependencyProperties.DependencyTypeName}}} data {{{DependencyProperties.Data}}} finished in {{{DependencyProperties.DurationInMs}}} ms (success: {{{DependencyProperties.Success}}}) ({{{DependencyProperties.StartTime}}})",
                "ServiceFabric",
                serviceName,
                data,
                duration.TotalMilliseconds,
                success,
                started);
        }

        /// <summary>
        /// Extension for Logging requests
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="responseOptions"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <param name="requestOptions"></param>
        /// <returns></returns>
        public static void LogRequest(this ILogger logger, HttpRequestOption requestOptions, HttpResponseOption responseOptions, DateTime started, TimeSpan duration)
        {
            logger.LogInformation(ServiceFabricEvent.ApiRequest,
                $"The {{{ApiRequestProperties.Method}}} {{{ApiRequestProperties.Query}}} action to {{{ApiRequestProperties.Scheme}}}{{{ApiRequestProperties.Host}}}{{{ApiRequestProperties.Path}}} finished in {{{ApiRequestProperties.DurationInMs}}} ms with status code {{{ApiRequestProperties.StatusCode}}} Headers: {{{ApiRequestProperties.RequestHeaders}}} Body: {{{ApiRequestProperties.RequestBody}}} ResponseHeaders: {{{ApiRequestProperties.ResponseHeaders}}} ResponseBody: {{{ApiRequestProperties.ResponseBody}}} StartTime: ({{{ApiRequestProperties.StartTime}}} ErrorResponse: ({{{ApiRequestProperties.ErrorResponse}}})",
                requestOptions.Method,
                requestOptions.Query,
                requestOptions.Scheme,
                requestOptions.Host,
                requestOptions.Path,
                duration.TotalMilliseconds,
                responseOptions.StatusCode,
                requestOptions.Headers,
                requestOptions.Body,
                responseOptions.Headers,
                responseOptions.Body,
                started,
                responseOptions.ErrorResponse);
        }

        /// <summary>
        /// Extension for Logging Exception. USE IT for logging exceptions instead of LogCritical and LogError.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: <code>"User {User} logged in from {Address}"</code></param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogException(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.LogCritical(ServiceFabricEvent.Exception, exception, message, args);
        }
    }
}
