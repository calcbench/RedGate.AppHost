﻿using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CommandLine;
using RedGate.AppHost.Interfaces;

namespace RedGate.AppHost.Client
{
    internal class Options
    {
        [Option('a', "assembly", Required = true, HelpText = "Assembly that contains an IOutOfProcessEntryPoint to load")]
        public string Assembly { get; set; }

        [Option('i', "id", Required = true, HelpText = "The communication channel to call back to the host")]
        public string ChannelId { get; set; }

        [Option('d', "debug", Required = false, HelpText = "Opens the client in debug mode")]
        public bool Debug { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Red Gate Out of Process App Host");
            return usage.ToString();
        }
    }

    internal static class Program
    {
        private static SafeAppHostChildHandle s_SafeAppHostChildHandle;

        [STAThread]
        private static void Main(string[] args)
        {

            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
#if DEBUG
                options.Debug = true;
#endif

                if (options.Debug)
                    ConsoleNativeMethods.AllocConsole();
                
                
                MainInner(options.ChannelId, options.Assembly);
            }
            else
            {
                MessageBox.Show("Hello :)\n\nI'm the child process for the Red Gate Deployment Manager SSMS add-in.\n\nPlease use SSMS rather than running me directly.", "RedGate.SQLCI.UI", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static void MainInner(string id, string assembly)
        {
            var entryPoint = LoadChildAssembly(assembly);
            InitializeRemoting(id, entryPoint);
            SignalReady(id);
            RunWpf();
        }

        private static IOutOfProcessEntryPoint LoadChildAssembly(string assembly)
        {
            var outOfProcAssembly = Assembly.LoadFile(assembly);

            var entryPoint = outOfProcAssembly.GetTypes().Single(x => x.GetInterfaces().Contains(typeof (IOutOfProcessEntryPoint)));

            return (IOutOfProcessEntryPoint) Activator.CreateInstance(entryPoint);
        }

        private static void InitializeRemoting(string id, IOutOfProcessEntryPoint entryPoint)
        {
            Remoting.Remoting.RegisterChannels(true, id);

            s_SafeAppHostChildHandle = new SafeAppHostChildHandle(Dispatcher.CurrentDispatcher, entryPoint);
            Remoting.Remoting.RegisterService<SafeAppHostChildHandle, ISafeAppHostChildHandle>(s_SafeAppHostChildHandle);
        }

        private static void SignalReady(string id)
        {
            using (EventWaitHandle signal = EventWaitHandle.OpenExisting(id))
            {
                signal.Set();
            }
        }

        private static void RunWpf()
        {
            Dispatcher.Run();
        }
    }
}
