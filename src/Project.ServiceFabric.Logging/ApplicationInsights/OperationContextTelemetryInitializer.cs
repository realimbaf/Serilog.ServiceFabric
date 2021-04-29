using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Project.ServiceFabric.Logging.ApplicationInsights
{
    internal class OperationContextTelemetryInitializer : ITelemetryInitializer
    {
        private readonly Func<string> _operationIdProvider;

        public OperationContextTelemetryInitializer(Func<string> operationIdProvider)
        {
            this._operationIdProvider = operationIdProvider;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Operation.Id = _operationIdProvider.Invoke();
            telemetry.Context.Operation.ParentId = _operationIdProvider.Invoke();

            if(telemetry.Context.Operation.Name == null)
                telemetry.Context.Operation.Name = Guid.NewGuid().ToString();
        }
    }
}