using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;

namespace BluSnapin.Cmdlet
{
    // BluSnapin Invoke-ChefRequest Cmdlet   
    [Cmdlet(VerbsLifecycle.Invoke, "ChefRequest")]
    public partial class Invoke_ChefRequest : PSCmdlet
    {
        /// <summary>
        /// Main Processing function of the CmdLet
        /// </summary>
        protected override void ProcessRecord()
        {
            Host.UI.Write(ConsoleColor.DarkGray, Host.UI.RawUI.BackgroundColor, "DEBUG: Cmdlet started.");
        }
    }
}
