using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.ServiceFabric.Logging")]
namespace Project.ServiceFabric.Logging.Abstractions.LogProperties
{
    internal static class ApiRequestProperties
    {
        public const string Scheme = "Scheme";
        public const string Method = "Method";
        public const string Query = "Query";
        public const string Host = "Host";
        public const string Path = "Path";
        public const string DurationInMs = "DurationInMs";
        public const string StatusCode = "StatusCode";
        public const string StartTime = "StartTime";
        public const string RequestHeaders = "RequestHeaders";
        public const string RequestBody = "RequestBody";
        public const string ResponseHeaders = "ResponseHeaders";
        public const string ResponseBody = "ResponseBody";
        public const string ErrorResponse = "ErrorResponse";
    }
}
