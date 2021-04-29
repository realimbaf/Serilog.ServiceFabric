using System.Runtime.CompilerServices;

namespace Project.ServiceFabric.Logging.Abstractions.Constants
{
    /// <summary>
    /// Set of constants that identify headers in remoting calls
    /// </summary>
    public class HeaderIdentifiers
    {
        /// <summary>
        /// Name of the header that contains the trace id
        /// </summary>
        public const string TraceId = "X-Fabric-TraceId";

    }

    public class ContentTypes
    {
        /// <summary>
        /// Content type is Json
        /// </summary>
        public const string Json = "application/json";
    }
}
