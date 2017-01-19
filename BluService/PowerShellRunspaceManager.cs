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
            var userDomainKey = userData.User + "::" + userData.Domain;
            if (!_runspaces.ContainsKey(userDomainKey))
            {
                _runspaces[userDomainKey] = new PowerShellRunspace(userData);
            }
            try
            {
                return _runspaces[userDomainKey].ExecuteScriptBlock(scriptBlock);
            }
            catch (PowerShellRunspaceException err)
            {
                try
                {
                    _runspaces[userDomainKey].Dispose();
                }
                catch
                {
                    // no errors from here
                }
                _runspaces.Remove(userDomainKey);
                return err.Message;
            }
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
