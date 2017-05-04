using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blu.core.common
{
    public static class Config
    {
        public const string ServiceName = "BluService";
        public const string ExecuteCommand = "exec";
        public const string CreateRunspaceCommand = "create-runspace";
        public const string DisposeRunspaceCommand = "dispose-runspace";
        public const string MagicSplitString = "-:#:#:#:-";
        public const string ShellName = "BluShell";
        public const string PipeName = "BluPowerShell";
        public static string SeparatorLine = Environment.NewLine + 
                                             "------------------------------------------" +
                                             Environment.NewLine;

        public static string TempPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location ?? ""), "temp-scripts");
        public const string RunspaceExecutionDone = "execution-of-script-completed-@341!";
    }
}
