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
            return _runspaces[DefaultRunspace].ExecuteScriptBlock(scriptBlock);
        }

        public string ExecuteScriptBlock(string scriptBlock, PowerShellRunspace.UserData userData)
        {
            return ExecuteScriptBlock(scriptBlock, userData, 1);
        }

        private string ExecuteScriptBlock(string scriptBlock, PowerShellRunspace.UserData userData, int attempt)
        {
            var userDomainKey = userData.User + "::" + userData.Domain;
            if (!_runspaces.ContainsKey(userDomainKey))
            {
                try
                {
                    _runspaces[userDomainKey] = new PowerShellRunspace(userData);
                }
                catch
                {
                    if (attempt < 3)
                    {
                        Thread.Sleep(500);
                        ExecuteScriptBlock(scriptBlock, userData, ++attempt);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return _runspaces[userDomainKey].ExecuteScriptBlock(scriptBlock);
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
