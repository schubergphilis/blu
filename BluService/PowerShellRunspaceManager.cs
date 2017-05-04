using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Blu.core.common;

namespace BluService
{
    public class PowerShellRunspaceManager : IDisposable
    {
        private const string DefaultRunspace = "_default";
        private readonly Dictionary<string, PowerShellRunspace> _runspaces = new Dictionary<string, PowerShellRunspace>();

        public string ProcessMessage(string message)
        {
            var parts = GetMessageParts(message);
            var command = parts[0];
            switch (command)
            {
                case Config.CreateRunspaceCommand:
                    return CreateRunspace(parts);
                case Config.DisposeRunspaceCommand:
                    return DisposeRunspace(parts);
                case Config.ExecuteCommand:
                    return Execute(parts);
                default:
                    return "Exit1:UknownCommand";
            }
        }

        private string CreateRunspace(string[] parts)
        {
            switch (parts.Length)
            {
                case 2:
                    return CreateRunspace(parts[1], null);
                case 3:
                    var userData = new UserData(parts[1]);
                    return CreateRunspace(parts[2], userData);
                default:
                    var error = "Unexpected number of message parts to create runspace. Received " + parts.Length + " parts, expecting 2 or 3.";
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                    return "Exit1:" + error;
            }
        }

        private string CreateRunspace(string runspace, UserData userData)
        {
            if (_runspaces.ContainsKey(runspace))
            {
                return "Exit0:Runspace " + runspace + " already exists, command ignored.";
            }
            _runspaces[runspace] = new PowerShellRunspace(runspace, userData);
            return "Exit0:Runspace " + runspace + " created.";
        }

        private string DisposeRunspace(string[] parts)
        {
            if (parts.Length != 2)
            {
                var error = "Unexpected number of message parts to dispose runspace. Received " + parts.Length + " parts, expecting 2.";
                EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                return "Exit1:" + error;
            }
            RemoveRunspace(parts[1]);
            return "Exit0:Runspace " + parts[1] + " disposed.";
        }

        private string Execute(string[] parts)
        {
            switch (parts.Length)
            {
                case 2:
                    return ExecuteScript(parts[1]);
                case 3:
                    return ExecuteScript(parts[2], parts[1]);
                default:
                    var error = "Unexpected number of message parts. Received " + parts.Length + " parts, expecting 2 or 3.";
                    EventLogHelper.WriteToEventLog(EventLogEntryType.Error, error);
                    return "Exit1:" + error;
            }
        }

        private string[] GetMessageParts(string message)
        {
            return message.Split(new[] { Config.MagicSplitString }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string ExecuteScript(string scriptFile)
        {
            if (!_runspaces.ContainsKey(DefaultRunspace))
            {
                _runspaces[DefaultRunspace] = new PowerShellRunspace(DefaultRunspace);
            }
            return ExecuteScript(scriptFile, DefaultRunspace);
        }

        private string ExecuteScript(string scriptFile, string runspace)
        {
            if (!scriptFile.EndsWith(".ps1") && !File.Exists(scriptFile))
            {
                return "Exit1:Invalid script file " + scriptFile;
            }

            try
            {
                if (!_runspaces.ContainsKey(runspace))
                {
                    return "Exit1:Runspace " + runspace +
                           " has not been created yet, ABORTING. Please execute BluShell runspace " + runspace +
                           " --credentials <credentials> first.";
                }
                return _runspaces[runspace].ExecuteScript(scriptFile);
            }
            catch (Exception err)
            {
                RemoveRunspace(runspace);
                return "Exit1:ERROR executing script: " + scriptFile + Environment.NewLine + "Runspace: " + runspace + Environment.NewLine + "Exception: " + err;
            }
        }

        private void RemoveRunspace(string runspace)
        {
            if (!_runspaces.ContainsKey(runspace))
            {
                return;
            }
            try
            {
                _runspaces[runspace].Dispose();
            }
            catch
            {
                // no errors from here
            }
            _runspaces.Remove(runspace);
        }

        public void Dispose()
        {
            foreach (var runspace in _runspaces.Values)
            {
                runspace.Dispose();
            }
        }
    }
}
