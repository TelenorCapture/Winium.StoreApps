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
                Console.WriteLine("Dependency : " + options.Dependency);
            }
            else
            {
                Console.WriteLine("No dependency");
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
