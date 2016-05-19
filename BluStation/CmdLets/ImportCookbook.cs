using System;
using System.Reflection;
using System.Management.Automation;
using BluApi.Chef.ChefAPI;
using BluApi.Chef.ChefResources;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.CmdLets
{
    // BluStation Get-Cookbook method   
    [Cmdlet(VerbsData.Import, "Cookbook")]
    public class ImportCookbook : PSCmdlet
    {
        /// <summary>
        /// -Name parameter: the name of cookbook
        /// </summary>   
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// -Version parameter: version of cookbook
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Version { get; set; }

        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();
            // ChefConfig.apiLog = false;

            // Default -Version to _latest
            if (Version == null) Version = "_latest";

            Cookbook cookbook = new Cookbook();
            ReturnType rt = cookbook.Download(Name, Version);

            if (rt.Result == 0)
            {
                Logger.log("ok", "Added cookbook: " + Name + "[" + Version + "] to cookbook path.");
            }
            else
            {
                Logger.log("error", rt.Message);
                Logger.log("error", "There is an error adding cookbook: " + Name + "[" + Version + "].");
                Terminate(rt.Message);
            }
        }

        private void Terminate(string badNews)
        {
            var errorRecord = new ErrorRecord(new Exception(badNews), badNews, ErrorCategory.CloseError, null);
            ThrowTerminatingError(errorRecord);
        }
    }
}

