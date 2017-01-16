using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Dispose()
        {
            foreach (var runspace in _runspaces.Values)
            {
                runspace.Dispose();
            }
        }
    }
}
