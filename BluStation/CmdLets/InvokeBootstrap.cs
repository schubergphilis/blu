using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Management.Automation;
using BluApi.Chef.ChefAPI;
using BluApi.Chef.ChefResources;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;


namespace BluStation.CmdLets
{
    /// <summary>
    /// Invoke-Bootstap class: a CmdLet to Bootstrap a machine
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "Bootstrap")]
    public class InvokeBootstrap : PSCmdlet
    {
        /// <summary>
        /// -Org parameter: Uri for Chef organization
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Org { get; set; }

        // v_Org is validator function for Org parameter. 
        public Function ValidateOrg()
        {
            ReturnType rt = new ReturnType();
            // Check if Uri is absolute
            if (!Uri.IsWellFormedUriString(Org, UriKind.Absolute))
            {
                rt.Result = 4;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Uri is not well formatted. Check the provided Uri in your browser.";
                return rt;
            }

            // Check if Uri can be converted to System.Uri
            try
            {
                ChefConfig.OrganizationUri = new Uri(Org);
            }
            catch (Exception ex)
            {
                rt.Result = 4;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Org Parameter is invalid. Can not convert " + Org + " to System.Uri. " + ex.Message + " Check the provided Uri in your browser.";
                return rt;
            }

            // Check if we can extract organization name from Uri
            string organization = Org.Substring(Org.LastIndexOf("/", StringComparison.Ordinal) + 1, Org.Length - Org.LastIndexOf("/", StringComparison.Ordinal) - 1);
            if (organization == "")
            {
                rt.Result = 4;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Organization name is empty or Uri contains trailing slashes. Remove trailing slashes from the Organization Uri.";
                return rt;
            }
            else
            {
                ChefConfig.Organization = organization;    // Organization Name is set
                ChefConfig.OrganizationUri = new Uri(Org);     // Organization Uri is set
                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Using Organization Name: " + ChefConfig.Organization + " and Uri: " + ChefConfig.OrganizationUri;
                return rt;
            }
        }

        // ══════════════════════════
        //  -Node Parameter
        // ══════════════════════════

        /// <summary>
        /// -Node parameter: Chef nodeName
        /// </summary> 
        [Parameter(Mandatory = true)]
        public string Node { get; set; }

        public Function ValidateNode()
        {
            ReturnType rt = new ReturnType();

            // Setting ChefConfig.nodeName
            ChefConfig.NodeName = Node;

            if (Node.Length <= 3)
            {
                // Warn if nodeNames is shorter than 3 letters
                rt.Result = 2;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Node name is shorter than 3 letters.";
            }
            else
            {
                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Using Node Name: " + ChefConfig.NodeName;
            }
            return rt;
        }

        // ══════════════════════════
        //  -Validator Parameter
        // ══════════════════════════

        /// <summary>
        /// -Node parameter: Chef nodeName
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Validator { get; set; }

        public Function ValidateValidator() // funny name: we are validating if the parameter -Validator is correct :)
        {
            ReturnType rt = new ReturnType();

            if (Validator != null)
            {
                ChefConfig.Validator = Validator;
                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Using Validator Name: " + ChefConfig.Validator;

            }
            else
            {
                ChefConfig.Validator = ChefConfig.Organization + "-validator";
                rt.Result = 1;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Validator parameter is not provided, switching to default validator: " + ChefConfig.Validator;

            }
            return rt;
        }

        // ═════════════════════════════
        //  -KeyPath and -Key Parameters
        // ═════════════════════════════

        /// <summary>
        /// -KeyPath parameter: the path to the validation key (.pem) file
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string KeyPath { get; set; }

        /// <summary>
        /// -Key parameter: client RSA key (pem) as plain string, used in Terraform plan
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Key { get; set; }


        public Function ValidateKey()
        {
            ReturnType rt = new ReturnType();
            
            // KeyPath is not provided.
            if (KeyPath == null)
            {
                if (File.Exists(ChefConfig.Root + @"\validation.pem"))
                {
                    ChefConfig.ClientPem = ChefConfig.Root + @"\validation.pem";
                    rt.Result = 0;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "Key Parameter is not provided, switching to default: " + ChefConfig.ClientPem;
                }
                else
                {
                    rt.Result = 4;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "Key Parameter is not provided and default .\\validation.pem file can not be found.";
                }
            }
            // KeyPath is provided.
            else
            {
                if (File.Exists(KeyPath))
                {
                    ChefConfig.ClientPem = KeyPath;
                    ChefConfig.ValidationKey = "UNSET"; // Unset validation key as string, we are using .pem file reader.
                    rt.Result = 0;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "Using RSA Key : " + ChefConfig.ClientPem;
                }
                else if (File.Exists(ChefConfig.Root + "\\" + KeyPath))
                {
                    ChefConfig.ClientPem = ChefConfig.Root + "\\" + KeyPath;
                    ChefConfig.ValidationKey = "UNSET";
                    rt.Result = 0;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "Using RSA Key : " + ChefConfig.ClientPem;
                }
                else
                {
                    rt.Result = 4;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "RSA Key file " + KeyPath + " not found.";
                }
            }

            // Tried all KeyPath but not found the file, let's try the -Key parameter
            if (rt.Result == 4)
            {
                if (Key != null &&
                Key.Contains("-----BEGIN RSA PRIVATE KEY-----") &&
                Key.Contains("-----END RSA PRIVATE KEY-----") &&
                Key.Length > 1000
                )
                {
                    rt.Result = 0;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "RSA Key seems to be OK, let's try using this Key.";
                    ChefConfig.ClientPem = "UNSET"; // Unset .pem path, we are using RSA key as string.
                    ChefConfig.ValidationKey = Key;
                }
                else
                {
                    // Unset both and don't bother signing and sending a request to Chef server.
                    ChefConfig.ClientPem = "UNSET";
                    ChefConfig.ValidationKey = "UNSET";
                    rt.Result = 4;
                    rt.Data = String.Empty;
                    rt.Object = null;
                    rt.Message = "RSA Key is not correctly formatted and KeyPath is also not helping much. Please check your RSA key.";
                }
            }
            return rt;
        }

        // ════════════════════════════════
        //  Env, Runlist and Roles
        // ════════════════════════════════

        /// <summary>
        /// -Env parameter: the name of Chef environment, like so: -Env green
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Env { get; set; }

        /// <summary>
        /// -RunList parameter: comma delimited runlist, like so: -RunList cookbak,cookbik::reciple,cookbax::recipka
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string RunList { get; set; }

        /// <summary>
        /// -Roles parameter: comma delimited roles, like so: -Roles royce,rock
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Roles { get; set; }

        // ════════════════════════════════
        //  Sync, Sudo and Force
        // ════════════════════════════════

        /// <summary>
        /// -Sync: Synchronize NodeName and Windows ComputerName
        /// </summary> 
        [Parameter(Mandatory = false)]
        public SwitchParameter Sync
        {
            get { return _sync; }
            set { _sync = value; }
        }
        private bool _sync;

        /// <summary>
        /// -Sudo parameter: the Soduer Name who runs bootstrap, like so: 
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Sudo { get; set; }

        /// <summary>
        /// -Force: overwrite client.rb, chef client and node
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get { return _force; }
            set { _force = value; }
        }
        private bool _force;


        // ══════════════════════════
        // Start Process
        // ══════════════════════════

        /// <summary>
        /// ProcessRecord: if mandatory parameters are present, process CmdLet
        /// </summary> 
        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            Logger.log("start", "Invoke-Bootstrap");

            // Validating Parameters, logging and exit process if validation returns FATAL
            ReturnType rt = ValidateOrg();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateNode();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateValidator();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateKey();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            Bootstrap();
        }

        /// <summary>
        /// Bootstrap the machine providing the mandatory parameters
        /// </summary>
        public void Bootstrap()
        {
            // ══════════════════════════
            //  Check client.rb
            // ══════════════════════════

            ChefConfig.ClientRb = ChefConfig.Root + @"\client.rb";

            if (File.Exists(ChefConfig.ClientRb))
            {
                if (_force)
                {
                    Logger.log("warn", "client.rb file exists and -Force parameter is true. client.rb file will be overwritten.");
                    File.Delete(ChefConfig.ClientRb);
                }
                else
                {
                    const string badNews = "client.rb file exists. Do not Invoke-Bootstrap if chef-client is already configured.";
                    Logger.log("fatal", badNews);
                    Terminate(badNews);
                }
            }
            
            // ══════════════════════════
            //  Add Client
            // ══════════════════════════
            
            Client client = new Client();
            ReturnType rt = client.Add(ChefConfig.NodeName, _force);
            
            if (rt.Result == 0)
            {
                using (StreamWriter sw = new StreamWriter(ChefConfig.Root + @"\client.pem", false))
                {
                    sw.WriteLine(rt.Data);
                }
                Logger.log("ok", "Chef client is succesfully created. RSA key is saved as client.pem");
            }
            else
            {
                Logger.log("fatal", rt.Message);
                Terminate(rt.Message);
            }

            // ══════════════════════════
            //  Create client.rb
            // ══════════════════════════

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"chef_server_url", "\"" + ChefConfig.OrganizationUri + "\""},
                {"node_name", "\"" + ChefConfig.NodeName + "\""}
            };

            try
            {
                StreamWriter sw = new StreamWriter(ChefConfig.ClientRb);
                foreach (var param in dict)
                {
                    sw.WriteLine("{0} {1}", param.Key, param.Value);
                }
                sw.Close();
                Logger.log("ok", "Client.rb file is succesfully created.");
            }
            catch (Exception ex)
            {
                string badNews = "Error writing client.rb file: " + ex.Message;
                Logger.log("fatal", badNews);
                Terminate(badNews);
            }
            
            // ══════════════════════════
            //  Add Node
            // ══════════════════════════            

           
            Node node = new Node();

            List<string> runlist = new List<string>();
            List<string> roles = new List<string>();
            
            if (RunList != null)
            {
                runlist = RunList.Replace(" ", String.Empty).Split(',').ToList();
            }
            if (Roles != null)
            {
                roles = Roles.Replace(" ", String.Empty).Split(',').ToList();
            }

            rt = node.Add(ChefConfig.NodeName, Env, runlist, roles, _force);

            if (rt.Result == 0)
            {
                Logger.log("ok", "Chef Node is succesfully created.");
            }
            else
            {
                Logger.log("fatal", rt.Message);
                Terminate(rt.Message);
            }

            // ══════════════════════════
            //  Create and Save Config
            // ══════════════════════════  

            // Adding ChefConfig parameters, needed for the next API connection using machine client account:
            ChefConfig.ClientName = ChefConfig.NodeName;
            ChefConfig.ClientPath = ChefConfig.Root;
            ChefConfig.ClientPem = ChefConfig.Root + @"\client.pem";
            
            // Saving config
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            rt = chefConfigurator.SaveConfig();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);
        }

        private void Terminate(string badNews)
        {
            var errorRecord = new ErrorRecord(new Exception(badNews), badNews, ErrorCategory.CloseError, null);
            ThrowTerminatingError(errorRecord);
        }
    }
}
