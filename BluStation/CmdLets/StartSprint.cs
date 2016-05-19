using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using BluStation.BluSprint;
using ReturnType = BluApi.Common.Function;


namespace BluStation.CmdLets
{
    // BluStation Invoke-Chefrun method   
    [Cmdlet(VerbsLifecycle.Start, "Sprint")]
    public class StartSprint : PSCmdlet
    {
        private Pipeline _pipeline;

        /// <summary>
        /// -Mode parameter: supported values are LWRP, StandAlone, Dev
        /// Default is StandAlone
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Mode { get; set; }

        /// <summary>
        /// -NodeObjext parameter: NodeOject as Json object
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string NodeObject { get; set; }
        

        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();

            // Disable API log to console 
            ChefConfig.ApiLog = false; 
            
            // Start Sprint
            ExecuteSprint();
        }
        
        public void ExecuteSprint()
        {
            ReturnType rt;
            Sprint sprint = new Sprint();

            //  ══════════════════════════
            //   Starting Blu
            //  ══════════════════════════
            Host.UI.Write(ConsoleColor.DarkGray, Host.UI.RawUI.BackgroundColor, ChefConfig.BluLogo);
            Console.WriteLine();
            Console.WriteLine();

            // Set Mode
            if (string.IsNullOrEmpty(Mode)) Mode = "StandAlone";
            sprint.Mode = Mode;

            // Check and save NodeObject
            if (Mode.ToUpper() == "LWRP")
            {
                if (string.IsNullOrEmpty(NodeObject))
                {
                    Terminate("NodeObject can not be empty when using LWRP mode.");
                }
                else
                {
                    try
                    {
                        File.WriteAllText(NodeObject, SprintData.NodeObject);
                    }
                    catch (Exception ex)
                    {
                        Terminate("Error writing node_object.json: " + ex.Message);
                    }
                }
            }

            // Delete the compiled script if exists
            if (File.Exists(SprintData.CompiledSprint)) 
                File.Delete(SprintData.CompiledSprint);

            // Delete the compiled resources if exists
            if (File.Exists(SprintData.CompiledResources)) 
                File.Delete(SprintData.CompiledResources);


            sprint.Build();

            string compiledScript = String.Empty;
            if (File.Exists(SprintData.CompiledSprint))
            {
                compiledScript = File.ReadAllText(SprintData.CompiledSprint);
            }
            else
            {
                Terminate("Compiled script (Spring.ps1) does not exist or is not readable.");
            }

            _pipeline = Runspace.DefaultRunspace.CreateNestedPipeline("New-Pipeline", true);
            _pipeline.Commands.AddScript(compiledScript);
            _pipeline.Output.DataReady += outputHandler;
            _pipeline.Invoke();
        }

        private void outputHandler(object sender, EventArgs e)
        {
            var errors = _pipeline.Error;
            var outputs = _pipeline.Output.NonBlockingRead();
            if (errors != null && errors.Count > 0)
            {
                Logger.log("error", errors.ToString());
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("_______________________PIPELINE EXCEPTION________________________");
                Console.WriteLine("  This generic error might not accurately report the exception.");
                Console.WriteLine("  You can execute the compiled script by:");
                Console.WriteLine("  PS> cd " + SprintData.RuntimePath);
                Console.WriteLine("  PS> .\\Sprint.ps1");
                Console.WriteLine("  That gives you more insight about what exactly is going wrong.");
                Console.WriteLine("_________________________________________________________________");
                Console.WriteLine();
                Console.WriteLine();
                Terminate(errors.ToString());
            }
            foreach (var output in outputs.Where(output => output != null))
            {
                WriteObject(output);
            }
        }

        private void Terminate(string badNews)
        {
            var errorRecord = new ErrorRecord(new Exception(badNews), badNews, ErrorCategory.CloseError, null);
            ThrowTerminatingError(errorRecord);
        }
    }
}
