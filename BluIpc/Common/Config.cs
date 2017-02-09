using System;

namespace BluIpc.Common
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
    }
}
