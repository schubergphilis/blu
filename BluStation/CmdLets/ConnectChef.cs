using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Management.Automation;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.CmdLets
{
    /// <summary>
    /// BluStation Connect-Chef method
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "Chef")]
    public class ConnectChef : PSCmdlet
    {
        /// <summary>
        /// -Org parameter: Uri for Chef organization
        /// </summary>  
        [Parameter(Mandatory = false)]
        public string Org { get; set; }

        /// <summary>
        /// ValidateOrg is validator function for Org parameter.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// -Client parameter: Chef clientName
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Client { get; set; }

        /// <summary>
        /// Sets ChefConfig.Validator and ChefConfig.ClientName 
        /// </summary>
        /// <returns>RT: rt.Result 0 as result and rt.Message </returns>
        public Function ValidateClient()
        {
            ReturnType rt = new ReturnType();
            ChefConfig.Validator = Client;
            ChefConfig.ClientName = Client;
            rt.Result = 0;
            rt.Data = String.Empty;
            rt.Object = null;
            rt.Message = "Using Client Name: " + ChefConfig.ClientName;
            return rt;
        }

        /// <summary>
        /// -Node parameter: Chef nodeName
        /// </summary> 
        [Parameter(Mandatory = false)]
        public string Node { get; set; }

        /// <summary>
        /// Sets ChefConfig.Validator and ChefConfig.ClientName 
        /// </summary>
        /// <returns>RT: rt.Result 0 as result and rt.Message </returns>
        public Function ValidateNode()
        {
            ReturnType rt = new ReturnType();
            ChefConfig.NodeName = Node;
            rt.Result = 0;
            rt.Data = String.Empty;
            rt.Object = null;
            rt.Message = "Using Node Name: " + ChefConfig.NodeName;
            return rt;
        }


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


        /// <summary>
        /// Either -Key or -Key file should be used and validated
        /// </summary>
        /// <returns>Multi type (ReturnType)</returns>
        public Function ValidateKey()
        {
            ReturnType rt = new ReturnType();

            // KeyPath is not provided.
            if (KeyPath == null)
            {
                if (File.Exists(ChefConfig.Root + @"\client.pem"))
                {
                    ChefConfig.ClientPem = ChefConfig.Root + @"\client.pem";
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
                    rt.Message = "Key Parameter is not provided and default .\\client.pem file can not be found.";
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

            // Tried all KeyPath but not found the file, let's try -Key parameter
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

        /// <summary>
        /// Main Processing function of the CmdLet
        /// </summary>
        protected override void ProcessRecord()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => Assembly.Load(AssemblyResolver.ResolveAssembly(args));
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            ReturnType rt = chefConfigurator.LoadConfig();

            if (rt.Result == 0)
            {
                // Configuration is loaded from registry, try to connect
                Connect();
            }
            else
            {
                ProcessConfiguration();
            }
        }

        private void ProcessConfiguration()
        {
            // Validating Parameters, logging and exit process if validation returns FATAL
            ReturnType rt = ValidateOrg();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateClient();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateNode();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            rt = ValidateKey();
            Logger.log(rt);
            if (rt.Result == 4) Terminate(rt.Message);

            Connect();
        }

        public void Connect()
        {
            // Check client connection
            Dictionary<string, dynamic> client = (Dictionary<string, dynamic>)ChefEndpoint.Get("clients/" + ChefConfig.ClientName, "/").Object;
            if (client.Count > 0)
            {
                // Show logo
                Host.UI.Write(ConsoleColor.DarkGray, Host.UI.RawUI.BackgroundColor, ChefConfig.BluLogo);
                Console.WriteLine();
                Logger.log("ok", "Connected!");
                Logger.log("info", "Client Name: " + ChefConfig.ClientName + " / Organization: " + ChefConfig.Organization);
                
                // Save Config
                ChefConfigurator chefConfigurator = new ChefConfigurator();
                chefConfigurator.SaveConfig();

                // Check if node exist
                Dictionary<string, dynamic> node = (Dictionary<string, dynamic>)ChefEndpoint.Get("nodes/" + ChefConfig.NodeName, "/").Object;
                if (node.Count > 0)
                {
                    Logger.log("ok", "Recieved Node json for: " + ChefConfig.NodeName);
                }
                else
                {
                    Console.WriteLine();
                    Logger.log("error", "Unable to retrieve Node data.");
                    Logger.log("info", "You are able to query Chef server by Use-ChefAPI CmdLet, but it seems the Chef node name you provided does not exist.");
                }
            }
            else
            {
                Console.WriteLine();
                string badNews = "Unable to connect as " + ChefConfig.ClientName + ".";
                Logger.log("error", badNews + " Please check if Chef client exists and the RSA key is valid.");
                Terminate(badNews);
            }

        }

        private void Terminate(string badNews)
        {
            var errorRecord = new ErrorRecord(new Exception(badNews), badNews, ErrorCategory.CloseError, null);
            ThrowTerminatingError(errorRecord);
        }
    }
}
