using System;
using System.Collections.Generic;

namespace Winium.StoreApps.Driver
{
    internal class Program
    {
        public static List<string> dependencyPaths = new List<string>();

        [STAThread]
        private static void Main(string[] args)
        {
            var listeningPort = 9999;

            var options = new CommandLineOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Port.HasValue)
                {
                    listeningPort = options.Port.Value;
                }
            }

            if (!string.IsNullOrEmpty(options.Dependency))
            {
               dependencyPaths.Add(options.Dependency);
                Console.WriteLine("\tDependency : " + options.Dependency);
            }
            else
            {
                const string vcppDependency = "C:\\Program Files (x86)\\Microsoft SDKs\\Windows Phone\v8.1\\ExtensionSDKs\\Microsoft.VCLibs\\12.0\\AppX\\Debug\\x86\\Microsoft.VCLibs.x86.Debug.12.00.Phone.appx";
                dependencyPaths.Add("\tDependency " + vcppDependency);
                dependencyPaths.Add(vcppDependency);
            }


            if (options.LogPath != null)
            {
                Logger.TargetFile(options.LogPath, options.Verbose);
            }
            else
            {
                Logger.TargetConsole(options.Verbose);
            }

            try
            {
                var listener = new Listener(listeningPort);
                Listener.UrnPrefix = options.UrlBase;

                Console.WriteLine("Starting WindowsPhone Driver on port {0}\n", listeningPort);

                listener.StartListening();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to start driver: {0}", ex);
                throw;
            }
        }
    }
}
