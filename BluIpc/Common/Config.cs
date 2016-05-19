using System;

namespace BluIpc.Common
{
    public static class Config
    {
        public static string ServiceName = "BluService";
        public static string ShellName = "BluShell";
        public static string PipeName = "BluPowerShell";
        public static string SeparatorLine = Environment.NewLine + 
                                             "------------------------------------------" +
                                             Environment.NewLine;
    }
}
