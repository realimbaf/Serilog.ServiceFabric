using Microsoft.Extensions.Logging;

namespace Project.ServiceFabric.Logging
{
    /// <summary>
    /// Interface for logging which can writes logs in stateful/stateless service, actors, asp.net core applications 
    /// </summary>
    public interface ILoggerFactoryBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aiKey"></param>
        /// <returns></returns>
        ILoggerFactory CreateLoggerFactory(string aiKey);
    }
}