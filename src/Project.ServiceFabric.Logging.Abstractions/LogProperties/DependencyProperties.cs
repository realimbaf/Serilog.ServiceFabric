using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.ServiceFabric.Logging")]
namespace Project.ServiceFabric.Logging.Abstractions.LogProperties
{
    internal static class DependencyProperties
    {
        public const string Data = "Name";
        public const string DurationInMs = "DurationInMs";
        public const string Success = "Success";
        public const string DependencyTypeName = "DependencyTypeName";
        public const string Type = "Type";
        public const string StartTime = "StartTime";
    }
}
