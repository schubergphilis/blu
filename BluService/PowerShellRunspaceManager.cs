using System;
using System.Collections.Generic;
using System.Threading;

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
            if (scriptBlock.Trim().ToLower() == "disposerunspace")
            {
                RemoveRunspace(runspace);
                return "Runspace " + runspace + " cleared";
            }
            try
            {
                return _runspaces[runspace].ExecuteScriptBlock(scriptBlock);
            }
            catch (PowerShellRunspaceException err)
            {
                RemoveRunspace(runspace);
                return err.Message;
            }
        }

        private void RemoveRunspace(string runspace)
        {
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
