using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Runtime;
using Project.ServiceFabric.Logging.Abstractions.Constants;
using Project.ServiceFabric.Logging.Abstractions.LogProperties;

namespace Project.ServiceFabric.Logging.Extensions
{
    /// <summary>
    /// Logger extension for service fabric dependent classes
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Extension for Logging dependencies
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="callMethod"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <param name="success"></param>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public static void LogDependency<TService, TResult>(this ILogger logger,
            Expression<Func<TService, Task<TResult>>> callMethod, DateTime started, TimeSpan duration, bool success)
            where TService : IService
        {
            logger.LogInformation(ServiceFabricEvent.ServiceRequest,
                $"The call to {{{DependencyProperties.Type}}} dependency {{{DependencyProperties.DependencyTypeName}}} named {{{DependencyProperties.Data}}} finished in {{{DependencyProperties.DurationInMs}}} ms (success: {{{DependencyProperties.Success}}}) ({{{DependencyProperties.StartTime}}})",
                "ServiceFabric",
                typeof(TService).FullName,
                ((MethodCallExpression)callMethod.Body).Method.ToString(),
                duration.TotalMilliseconds,
                success,
                started);
        }

        /// <summary>
        /// Extension for Logging dependencies
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="callMethod"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <param name="success"></param>
        /// <typeparam name="TService"></typeparam>
        public static void LogDependency<TService>(this ILogger logger, Expression<Func<TService, Task>> callMethod,
            DateTime started, TimeSpan duration, bool success) where TService : IService
        {
            logger.LogInformation(ServiceFabricEvent.ServiceRequest,
                $"The call to {{{DependencyProperties.Type}}} dependency {{{DependencyProperties.DependencyTypeName}}} named {{{DependencyProperties.Data}}} finished in {{{DependencyProperties.DurationInMs}}} ms (success: {{{DependencyProperties.Success}}}) ({{{DependencyProperties.StartTime}}})",
                "ServiceFabric",
                typeof(TService).FullName,
                ((MethodCallExpression)callMethod.Body).Method.ToString(),
                duration.TotalMilliseconds,
                success,
                started);
        }

        /// <summary>
        /// Extension for Logging dependencies
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        /// <param name="method"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <param name="success"></param>
        /// <param name="actorId"></param>
        public static void LogDependency(this ILogger logger, string service, string method,
            DateTime started, TimeSpan duration, bool success, ActorId actorId)
        {
            logger.LogInformation(ServiceFabricEvent.ServiceRequest,
                $"The call to {{{DependencyProperties.Type}}} actor {{{DependencyProperties.DependencyTypeName}}} with id {{{nameof(ActorId)}}} named {{{DependencyProperties.Data}}} finished in {{{DependencyProperties.DurationInMs}}} ms (success: {{{DependencyProperties.Success}}}) ({{{DependencyProperties.StartTime}}})",
                "ServiceFabric",
                service,
                actorId.ToString(),
                method,
                duration.TotalMilliseconds,
                success,
                started);
        }

        /// <summary>
        /// Extension for Logging method of actors
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="started"></param>
        /// <param name="duration"></param>
        /// <typeparam name="TActor"></typeparam>
        public static void LogActorMethod<TActor>(this ILogger logger, ActorMethodContext context, DateTime started, TimeSpan duration) where TActor : Actor
        {
            logger.LogInformation(ServiceFabricEvent.ActorMethod,
                "The {CallType} call to actor {Actor}  method {MethodName} finished in {Duration} ms (started at ({Started}))",
                context.CallType,
                typeof(TActor).FullName,
                context.MethodName,
                duration.TotalMilliseconds,
                started);
        }

        /// <summary>
        /// Extension for Logging listeners in stateless services
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="endpoint"></param>
        /// <typeparam name="T"></typeparam>
        public static void LogStatelessServiceStartedListening<T>(this ILogger logger, string endpoint) where T : StatelessService
        {
            logger.LogInformation(ServiceFabricEvent.ServiceListening,
                "The stateless service {ServiceType} started listening (endpoint {Endpoint})", typeof(T).FullName, endpoint);
        }

        /// <summary>
        /// Extension for Logging listeners in stateful services
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="endpoint"></param>
        /// <typeparam name="T"></typeparam>
        public static void LogStatefulServiceStartedListening<T>(this ILogger logger, string endpoint) where T : StatefulService
        {
            logger.LogInformation(ServiceFabricEvent.ServiceListening,
               "The stateful service {ServiceType} started listening (endpoint {Endpoint})", typeof(T).FullName, endpoint);
        }

        /// <summary>
        /// Extension for Logging initialization of stateless service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        public static void LogStatelessServiceInitializationFailed<T>(this ILogger logger, Exception exception) where T : StatelessService
        {
            logger.LogCritical(ServiceFabricEvent.ServiceInitializationFailed, exception,
                "The stateless service {ServiceType} failed to initialize.", typeof(T).FullName);
        }

        /// <summary>
        /// Extension for Logging initialization of stateful service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        public static void LogStatefulServiceInitializationFailed<T>(this ILogger logger, Exception exception) where T : StatefulService
        {
            logger.LogCritical(ServiceFabricEvent.ServiceInitializationFailed, exception,
                "The stateful service {ServiceType} failed to initialize.", typeof(T).FullName);
        }

        /// <summary>
        /// Extension for Logging initialization of actor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        public static void LogActorHostInitializationFailed<T>(this ILogger logger, Exception exception) where T : Actor
        {
            logger.LogCritical(ServiceFabricEvent.ActorHostInitializationFailed, exception,
                "The stateful service {ServiceType} failed to initialize.", typeof(T).FullName);
        }
    }
} 
