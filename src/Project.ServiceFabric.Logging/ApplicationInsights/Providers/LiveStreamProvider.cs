using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

namespace Project.ServiceFabric.Logging.ApplicationInsights.Providers
{
    internal class LiveStreamProvider
    {
        private readonly TelemetryConfiguration _configuration;

        internal LiveStreamProvider(TelemetryConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal void Enable()
        {
            var module = new DependencyTrackingTelemetryModule();
            module.Initialize(_configuration);

            QuickPulseTelemetryProcessor processor = null;

            _configuration.TelemetryProcessorChainBuilder
                .Use(next =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);
                    return processor;
                })
                .Build();

            var quickPulse = new QuickPulseTelemetryModule();
            quickPulse.Initialize(_configuration);
            quickPulse.RegisterTelemetryProcessor(processor);
        }
    }

}
