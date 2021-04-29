using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.ServiceFabric.Logging")]
namespace Project.ServiceFabric.Logging.Abstractions.LogProperties
{
    internal static class MetricProperties
    {
        public const string Name = "Name";
        public const string Value = "Value";
        public const string MinValue = "MinValue";
        public const string MaxValue = "MaxValue";
    }
}
