using Microsoft.AspNetCore.Builder;

namespace Project.ServiceFabric.Logging.Middleware
{
    /// <summary>
    /// Extension for registration <see cref="RequestTrackingMiddleware"/>
    /// </summary>
    public static class RequestTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTracking(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTrackingMiddleware>();
        }
    }
}