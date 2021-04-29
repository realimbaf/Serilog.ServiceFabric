using Microsoft.ApplicationInsights.Extensibility;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Providers
{
    internal static class LiveStreamProviderExtension
    {
        internal static TelemetryConfiguration EnableLiveStream(this TelemetryConfiguration configuration)
        {
            new LiveStreamProvider(configuration).Enable();
            return configuration;
        }
    }
}
