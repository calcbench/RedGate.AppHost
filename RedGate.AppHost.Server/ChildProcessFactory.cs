namespace RedGate.AppHost.Server
{
    public class ChildProcessFactory
    {
        public IChildProcessHandle Create(string assemblyName, bool openDebugConsole, bool is64Bit, bool monitorHostProcess, string executingDirectory = null)
        {
            IProcessStartOperation processStarter;

            if (is64Bit)
            {
                processStarter = new ProcessStarter64Bit();
            }
            else
            {
                processStarter = new ProcessStarter32Bit();
            }
            
            if (!string.IsNullOrEmpty(executingDirectory) )
            {
                processStarter.ExecutingDirectory = executingDirectory;
            }

            return new RemotedProcessBootstrapper(
                new StartProcessWithTimeout(
                    new StartProcessWithJobSupport(
                        processStarter))).Create(assemblyName, openDebugConsole, monitorHostProcess);
        }

        // the methods below are to support legacy versions of the API to the Create() method

        public IChildProcessHandle Create(string assemblyName, bool openDebugConsole, bool is64Bit)
        {
            return Create(assemblyName, openDebugConsole, is64Bit, false);
        }

        public IChildProcessHandle Create(string assemblyName, bool openDebugConsole)
        {
            return Create(assemblyName, openDebugConsole, false);
        }

        public IChildProcessHandle Create(string assemblyName)
        {
            return Create(assemblyName, false, false);
        }
    }
}
