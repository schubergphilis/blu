using System;
using System.IO;
using System.Reflection;
using System.Management.Automation;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.CmdLets
{
    // BluStation Get-Attributes method   
    [Cmdlet(VerbsOther.Use, "ChefAPI")]
    public class UseChefApi : PSCmdlet
    {
        /// <summary>
        /// -Select parameter
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Select { get; set; }

        /// <summary>
        /// -Path parameter
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Path { get; set; }

        /// <summary>
        /// -Format parameter
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Format { get; set; }

        /// <summary>
        /// -Secret parameter
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Secret { get; set; }

        /// <summary>
        /// -SecretFile parameter
        /// </summary>
        [Parameter(Mandatory = false)]
        public string SecretFile { get; set; }


        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();

            // Disable API log to console 
            // ChefConfig.apiLog = false; 

            // Default values for Node, Path and Format parameters
            if (Select == null)
            {
                if (string.IsNullOrEmpty(ChefConfig.NodeName))
                {
                    Select = "nodes/" + ChefConfig.NodeName;    
                }
                else
                {
                    Select = "nodes";
                }
            }
            else
            {
                // Trim the first / from -Endpoint
                Select = Select.TrimStart('/');
            }
            
            if (Format == null) Format = "DICTIONARY";
            if (Path == null) Path = "/";

            ReturnType rt = ChefEndpoint.Get(Select, Path);
            
            if (rt.Result == 0)
            {

                // Determine if data is encrypted
                string key = String.Empty;
                if (Secret != null)
                {
                    Format = "SECRET";
                    key = Secret;
                }
                else if (SecretFile != null)
                {
                    Format = "SECRET";
                    key = File.ReadAllText(ChefConfig.Root + "\\" + SecretFile);
                }

                switch (Format.ToUpper())
                {
                    case "JSON":
                        WriteObject(rt.Data);
                        break;

                    case "DICTIONARY":
                        WriteObject(rt.Object);
                        break;

                    default:
                        Logger.log("error", "Output format is not recognized. Accepted values are 'Json', 'Dictionary'");
                        Terminate("Unrecognized Format");
                        break;
                }
            }
            else
            {
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
