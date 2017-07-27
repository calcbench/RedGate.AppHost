using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RedGate.AppHost.Server
{
    internal abstract class ProcessStarter : IProcessStartOperation
    {
        protected abstract string ProcessFileName { get; }
        private string executingAssembly;
        public Process StartProcess(string assemblyName, string remotingId, bool openDebugConsole, bool monitorHostProcess)
        {
            string quotedAssemblyArg = "\"" + Path.Combine(ExecutingDirectory, assemblyName) + "\"";

            var processToStart = Path.Combine(ExecutingDirectory, ProcessFileName);
            var processArguments = string.Join(" ", new[]
            {
                "-i " + remotingId,
                "-a " + quotedAssemblyArg,
                openDebugConsole ? "-d" : string.Empty,
                monitorHostProcess ? "-p " + Process.GetCurrentProcess().Id : string.Empty
            });
            return Process.Start(processToStart, processArguments);
        }
        
        public string ExecutingDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(executingAssembly))
                {
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    return executingAssembly;
                }
            }
            set
            {
                executingAssembly = value;
            }
        }
    }
}
