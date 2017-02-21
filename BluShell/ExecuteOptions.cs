using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace BluShell
{
    [Verb("exec", HelpText = "Execute a powershell command or script via blu.")]
    class ExecuteOptions
    {
        [Option('s', "script-block", Required = false, HelpText = "Script to be executed")]
        public string ScriptBlock { get; set; }

        [Option('f', "script-file", Required = false, HelpText = "Script file to be executed.")]
        public string ScriptFile { get; set; }

        [Option('r', "runspace", Required = false, HelpText = "Execute command or script using a named runspace created with BluShell runspace [-c <credentials>] <name>.")]
        public string Runspace { get; set; }

        [Usage(ApplicationAlias = "BluShell")]
        public static IEnumerable<Example> Usage
        {
            get
            {
                yield return new Example("Execute a command", new ExecuteOptions { ScriptBlock = "$env:path" });
                yield return new Example("Execute a script", new ExecuteOptions { ScriptFile = "c:\\temp\\check-system.ps1" });
                yield return new Example("Execute a script using a named runspace", new ExecuteOptions { ScriptBlock = "c:\\temp\\check-system.ps1", Runspace = "admin" });
            }
        }
    }
}
