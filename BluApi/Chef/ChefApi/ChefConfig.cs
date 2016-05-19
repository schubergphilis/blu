using System;
using System.IO;
using System.Reflection;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Chef.ChefAPI
{
    public static class ChefConfig
    {
        /// <summary>
        /// Root is Assembly Location.
        /// </summary>
        public static string Root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// URI of the Chef organization.
        /// </summary>
        public static Uri OrganizationUri = new Uri("https://organizationUri_is_undefined");
        
        /// <summary>
        /// Chef organization name.
        /// </summary>
        public static string Organization = "";

        /// <summary>
        /// Name of the Chef client.
        /// </summary>
        public static string ClientName = "";
        
        /// <summary>
        /// Name of the Chef node.
        /// </summary>
        public static string NodeName = "";
        
        /// <summary>
        /// The Chef validator.
        /// </summary>
        public static string Validator = "";
        
        /// <summary>
        /// The validation key.
        /// </summary>
        public static string ValidationKey = "UNSET";

        /// <summary>
        /// Absolute path of the client directory (normally the current directory).
        /// </summary>
        public static string ClientPath = "";

        /// <summary>
        /// Absolute path of the client.rb file.
        /// </summary>
        public static string ClientRb = "";

        /// <summary>
        /// Absolute path of the client.pem file.
        /// </summary>
        public static string ClientPem = "UNSET";

        /// <summary>
        /// Determines if we log API messages.
        /// </summary>
        public static bool ApiLog = true;

        /// <summary>
        /// Absolute path to the cookbook development folder.
        /// </summary>
        public static string DevPath = "UNSET";

        /// <summary>
        /// The cookbook structure. Subject to update when this sturcture changes by Chef community.
        /// </summary>
        public static string[] CookbookStructure =
        {
        "recipes",
        "definitions",
        "libraries",
        "attributes",
        "files",
        "templates",
        "resources",
        "providers",
        "root_files"
        };

        /// <summary>
        /// Know Chef attributes names
        /// </summary>
        public static string[] KnownAttributeNames =
        {
        "node",
        "default",
        "override"
        };

        /// <summary>
        /// Logo in ascii art format
        /// </summary>
        public static string BluLogo = @"

  __          __    __     
  \ \        / /_  / /_  __
   \ \      / __ \/ / / / /
    \ \    / /_/ / / /_/ / 
     \_\  /_.___/_/\__,_/  
═══════════════════════════════";

    }
 
    /// <summary>
    /// Utility class to load and save Blu configuration to registry
    /// </summary>
    
    public class ChefConfigurator
    {
        /// <summary>
        /// Saves Blu configuration to registry HKLM\\Software\\Blu\\Config
        /// </summary>
        /// <returns></returns>
        public Function SaveConfig()
        {
            ReturnType rt = new ReturnType();
            RegistryHelper rh = new RegistryHelper { SubKey = "SOFTWARE\\Blu\\Config" };

            try
            {
                rh.Write("OrganizationUri", ChefConfig.OrganizationUri.ToString());
                rh.Write("Organization", ChefConfig.Organization);
                rh.Write("ClientName", ChefConfig.ClientName);
                rh.Write("NodeName", ChefConfig.NodeName);
                rh.Write("Validator", ChefConfig.Validator);
                rh.Write("ValidationKey", ChefConfig.ValidationKey);
                rh.Write("ClientPath", ChefConfig.ClientPath);
                rh.Write("ClientRb", ChefConfig.ClientName);
                rh.Write("ClientPem", ChefConfig.ClientPem);
                rh.Write("DevPath", ChefConfig.DevPath);

                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Configuration is saved to HKLM\\Software\\BluApi\\Config";
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Unable to save configuration to registry. Error: " + ex.Message;
            }
            return rt;
        }

        /// <summary>
        /// Loads Blu configuration from registry HKLM\\Software\\Blu\\Config
        /// </summary>
        /// <returns></returns>
        public Function LoadConfig()
        {
            ReturnType rt = new ReturnType();
            RegistryHelper rh = new RegistryHelper { SubKey = "SOFTWARE\\Blu\\Config" };

            try
            {
                ChefConfig.OrganizationUri = new Uri(rh.Read("OrganizationUri"));
                ChefConfig.Organization = rh.Read("Organization");
                ChefConfig.ClientName = rh.Read("ClientName");
                ChefConfig.NodeName = rh.Read("NodeName");
                ChefConfig.Validator = rh.Read("Validator");
                ChefConfig.ValidationKey = rh.Read("ValidationKey");
                ChefConfig.ClientPath = rh.Read("ClientPath");
                ChefConfig.ClientRb = rh.Read("ClientRb");
                ChefConfig.ClientPem = rh.Read("ClientPem");
                ChefConfig.DevPath = rh.Read("DevPath");

                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Configuration is loaded from HKLM\\Software\\BluApi\\Config";
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Unable to load configuration from registry. Error: " + ex.Message;
            }
            return rt;
        }
    }
}
