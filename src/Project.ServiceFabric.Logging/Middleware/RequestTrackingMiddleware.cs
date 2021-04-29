using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Project.Common.Extensions;
using Project.Common.Interfaces;
using Project.ServiceFabric.Logging.Abstractions;
using Project.ServiceFabric.Logging.Abstractions.Constants;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;
using Project.ServiceFabric.Logging.Abstractions.Models;
using Project.ServiceFabric.Logging.Extensions;
using ServiceFabric.Remoting.CustomHeaders;

namespace Project.ServiceFabric.Logging.Middleware
{
    /// <summary>
    /// Middleware for logging requests for AspNet Core application. 
    /// </summary>
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ITraceIdentifierProvider _traceIdentifierProvider;
        private readonly RecyclableMemoryStreamManager _recycableMemoryStreamManager;
        /// <summary>
        /// Constructor
        /// </summary>
        public RequestTrackingMiddleware(RequestDelegate next,
            ILogger logger,
            ITraceIdentifierProvider traceIdentifierProvider = null,
            RecyclableMemoryStreamManager recycableMemoryStreamManager = null)
        {
            _next = next;
            _logger = logger;
            _traceIdentifierProvider = traceIdentifierProvider;
            _recycableMemoryStreamManager = recycableMemoryStreamManager ?? new RecyclableMemoryStreamManager();
        }

        /// <summary>
        /// Invoke request tracking middleware
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, StatelessServiceContext serviceContext)
        {
            string traceIdentifier = _traceIdentifierProvider?.GetTraceIdentifer() ?? context.Request.HttpContext.TraceIdentifier;

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [SharedProperties.TraceId] = traceIdentifier
            }))
            {
                RemotingContext.SetData(HeaderIdentifiers.TraceId, traceIdentifier);

                AddTracingDetailsOnRequest(context, serviceContext);

                var stopwatch = Stopwatch.StartNew();
                DateTime started = DateTime.Now;
                HttpRequest request = context.Request;
                request.EnableBuffering();

                var requestOptions = new HttpRequestOption();
                try
                {
                    requestOptions.Scheme = request.Scheme;
                    requestOptions.Method = request.Method;
                    requestOptions.Query = request.QueryString.HasValue ? request.QueryString.Value : String.Empty;
                    requestOptions.Host = request?.Host.Value;
                    requestOptions.Path = request?.Path.Value;
                    requestOptions.Headers = request.ReadHeadersAsString();
                    requestOptions.Body = await request.ReadBodyAsStringAsync(_recycableMemoryStreamManager);
                }
                catch(Exception ex)
                {
                    _logger.LogException(ex, $"Unable to collect request options for logging request due to {ex.GetMessage()}");
                }
                
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, $"Error has been occured during executing nested middleware: {ex.GetMessage()}");
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    HttpResponse response = context.Response;

                    object errorResponse = null;
                    context.Items?.TryGetValue("Error-Response", out errorResponse);
                    
                    var responseOptions = new HttpResponseOption();
                    try
                    {
                        responseOptions.ErrorResponse = errorResponse?.ToString();
                        responseOptions.Body = await response.ReadBodyAsStringAsync(_recycableMemoryStreamManager);
                        responseOptions.Headers = response.ReadHeadersAsString();
                        responseOptions.StatusCode = response.StatusCode;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex, $"Unable to collect request options for logging request due to {ex.GetMessage()}");
                    }
                    _logger.LogRequest(requestOptions, responseOptions, started, stopwatch.Elapsed);
                }
            }

        }

        private static void AddTracingDetailsOnRequest(HttpContext context, ServiceContext serviceContext)
        {
            if (!context.Request.Headers.ContainsKey("X-Fabric-AddTracingDetails"))
            {
                return;
            }

            context.Response.Headers.Add("X-Fabric-NodeName", serviceContext.NodeContext.NodeName);
            context.Response.Headers.Add("X-Fabric-InstanceId", serviceContext.ReplicaOrInstanceId.ToString(CultureInfo.InvariantCulture));
            context.Response.Headers.Add("X-Fabric-TraceId", context.Request.HttpContext.TraceIdentifier);
        }
    }
}
