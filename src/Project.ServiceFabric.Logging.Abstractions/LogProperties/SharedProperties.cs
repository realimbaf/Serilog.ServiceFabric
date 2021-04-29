using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.ServiceFabric.Logging")]
namespace Project.ServiceFabric.Logging.Abstractions.LogProperties
{
    internal static class SharedProperties
    {
        public const string TraceId = "TraceId";
        public const string EventId = "EventId";
    }
}
