using System;
using System.Reflection;
using System.Management.Automation;
using BluApi.Common;

namespace BluStation.CmdLets
{
    /// <summary>
    /// BluStation New-Pipeline method
    /// It resolves the assembly names and starts new pipeline
    /// Invoking it from the command line directly has no restul 
    /// This CmdLet is used internally to invoke a new pipeline
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Pipeline")]
    public class NewPipeline : PSCmdlet
    {
        /// <summary>
        /// Reconrd processing only resolves the assembly names
        /// </summary>
        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
        }
    }
}
