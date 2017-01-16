using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace BluService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Install and uninstall service:
        /// sc create "Blu Powershell Runspace Service" binpath= D:\Github\BackslashBlu\BluService\bin\Release\BluService.exe
        /// sc description "Blu Powershell Runspace Service" "Provides a Runspace to execute PowerShell commands in service mode."
        /// sc delete "Blu Powershell Runspace Service"
        /// </summary>
        private static void Main(string[] args)
        {
            if (args.Contains("/standalone"))
            {
                var service = new BluService();
                service.Run();
                Console.WriteLine("Press Q to stop");
                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                    Thread.Sleep(100);
                }
                service.Stop();
            }
            else
            {
                ServiceBase[] bluService =
                {
                    new BluService()
                };
                ServiceBase.Run(bluService);
            }
        }
    }
}
