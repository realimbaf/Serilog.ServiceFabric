﻿using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Project.ServiceFabric.Logging")]
namespace Project.ServiceFabric.Logging.Abstractions.Constants
{
    internal static class FabricEnvironmentVariable
    {
        public static string ApplicationName => Environment.GetEnvironmentVariable("Fabric_ApplicationName");
        public static string ServicePackageName => Environment.GetEnvironmentVariable("Fabric_ServicePackageName");
        public static string ServicePackageInstanceId => Environment.GetEnvironmentVariable("Fabric_ServicePackageInstanceId");
        public static string ServicePackageActivationId => Environment.GetEnvironmentVariable("Fabric_ServicePackageActivationId");
        public static string NodeName => Environment.GetEnvironmentVariable("Fabric_NodeName");
    }
}
