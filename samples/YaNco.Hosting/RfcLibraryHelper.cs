using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Dbosoft.YaNco.Hosting
{
    [ExcludeFromCodeCoverage]
    public static class RfcLibraryHelper
    {
        /// <summary>
        /// This method ensures that either LD_LIBRARY_PATH or PATH points to base directory
        /// where referenced UCI libs should be stored.
        /// Please note that this don't work for docker containers
        /// </summary>
        public static void EnsurePathVariable()
        {
            var executableDir = AppDomain.CurrentDomain.BaseDirectory;
            var pathVariableName = "PATH";

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                pathVariableName = "LD_LIBRARY_PATH";

            var currentPathVariable = Environment.GetEnvironmentVariable(pathVariableName);

            if (currentPathVariable != null)
            {

                if (currentPathVariable.Contains(executableDir ?? throw new InvalidOperationException()))
                    return;

                if (!currentPathVariable.EndsWith(";"))
                    currentPathVariable += ';';

            }
            else
            {
                currentPathVariable = "";
            }


            Environment.SetEnvironmentVariable(pathVariableName, $"{currentPathVariable}{executableDir}");
        }
    }
}