using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BluIpc.Common;

namespace BluService
{
    public class PowerShellRunspaceManager : IDisposable
    {
        private const string DefaultRunspace = "_default";
        private readonly Dictionary<string, PowerShellRunspace> _runspaces;

        public PowerShellRunspaceManager()
        {
            _runspaces = new Dictionary<string, PowerShellRunspace>
            {
                {DefaultRunspace, new PowerShellRunspace()}
            };
        }

        public string ExecuteScriptBlock(string scriptBlock)
        {
            if (!_runspaces.ContainsKey(DefaultRunspace))
            {
                _runspaces[DefaultRunspace] = new PowerShellRunspace();
            }
            return ExecuteScriptBlock(scriptBlock, DefaultRunspace);
        }

        public string ExecuteScriptBlock(string scriptBlock, PowerShellRunspace.UserData userData)
        {
            var userDomainKey = userData.User + "::" + userData.Domain;
            if (!_runspaces.ContainsKey(userDomainKey))
            {
                _runspaces[userDomainKey] = new PowerShellRunspace(userData);
            }
            return ExecuteScriptBlock(scriptBlock, userDomainKey);
        }

        private string ExecuteScriptBlock(string scriptBlock, string runspace)
        {
            if (scriptBlock.EndsWith(".ps1") && File.Exists(scriptBlock))
            {
                var scriptFile = scriptBlock;
                try
                {
                    scriptBlock = LoadScriptFileIntoScriptBlock(scriptFile);
                }
                catch (Exception)
                {
                    return "Exit1:Cannot read: " + scriptFile;
                }
            }

            if (scriptBlock.Trim().ToLower() == "disposerunspace")
            {
                EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Removing runspace: " + runspace);
                RemoveRunspace(runspace);
                return "Exit0:Runspace " + runspace + " cleared";
            }
            try
            {
                return _runspaces[runspace].ExecuteScriptBlock(scriptBlock);
            }
            catch (Exception err)
            {
                RemoveRunspace(runspace);
                return "Exit1:ERROR executing script: " + scriptBlock + Environment.NewLine + "Runspace: " + runspace + Environment.NewLine + "Exception: " + err;
            }
        }

        private static string LoadScriptFileIntoScriptBlock(string scriptFile)
        {
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "Trying to read ps1 file: " + scriptFile);
            var content = File.ReadAllText(scriptFile)
                .TrimStart(' ')
                .TrimStart(Environment.NewLine.ToCharArray())
                .TrimEnd(' ')
                .TrimEnd(Environment.NewLine.ToCharArray());
            EventLogHelper.WriteToEventLog(EventLogEntryType.Information, "File content is: " + content);
            return content;
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
