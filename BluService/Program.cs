using System.ServiceProcess;

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
        static void Main()
        {
            ServiceBase[] bluService = { 
                new BluService() 
            };
            ServiceBase.Run(bluService);
        }
    }
}
