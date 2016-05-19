using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Chef.ChefResources
{
    public class Cookbook
    {
        public static string CookbookRoot = ChefConfig.Root + "\\runtime\\cookbooks";
        public List<string> ResourceList = new List<string>();
        public List<string> AttributeList = new List<string>(); 

        /// <summary>
        /// Download a cookbook
        /// </summary>
        /// <param name="cookbookName">Cookbook name</param>
        /// <param name="version">Cookbook version</param>
        /// <returns></returns>
        /// TODO: Retire this function if never used
        public Function Download(string cookbookName, string version)
        {
            return Download(cookbookName, version, String.Empty, String.Empty, false, false, false);
        }

        /// <summary>
        /// Download a cookbook
        /// </summary>
        /// <param name="cookbookName">Cookbook name</param>
        /// <param name="version">Cookbook version</param>
        /// <param name="parent">For recursive adding all dependencies</param>
        /// <param name="opr">Operator e.g >= or ~></param>
        /// <param name="rewrite">Rewrite the cookbook folder</param>
        /// <param name="onlyBlu">Download only if it is a Blu cookbook: a cookbook which have [supports 'blu'] in metadata</param>
        /// <param name="addToRunlist">Add cookbook to the runlist registy key</param>
        /// <returns></returns>
        public Function Download (
            string cookbookName,
            string version,
            string parent,
            string opr,
            bool rewrite,
            bool onlyBlu,
            bool addToRunlist
            )
        {
            ReturnType rt = new ReturnType();
            string cookbookPath = CookbookRoot + "\\" + cookbookName;

            // Set response to OK now. It will be overridden if there is an error
            rt.Result = 0;
            rt.Data = String.Empty;
            rt.Object = null;
            rt.Message = String.Empty;

            if (onlyBlu && !SupportsBlu(cookbookName))
            {
                
                throw new InvalidOperationException("Cookbook " + cookbookName + " does not suppport blu. Are you missing [supports 'blu'] directive in metadata?");
            }
           
            if (rewrite) cookbookPath.EmptyFolder();

            version = FindVersion(cookbookName, version, opr);
            Dictionary<string, dynamic> cookbook = CookbookToDictionary(cookbookName, version);

            // Loop through dictionary and find items
            foreach (KeyValuePair<string, dynamic> entry in cookbook)
            {
                // Check if item.key is found in cookbook structure (defined in ChefConfig.cookbookStructure)
                if (Array.IndexOf(ChefConfig.CookbookStructure, entry.Key) > -1)
                {
                    try
                    {
                        // Create a list of cookbook item which are valid to download
                        List<object> items = (List<object>)entry.Value;

                        // Put each List item into a dictionary
                        foreach (Dictionary<string, dynamic> item in items)
                        {
                            try
                            {
                                // Create each item path in filesystem
                                string itemPath = Path.GetDirectoryName(cookbookPath + "\\" + item["path"]);
                                if (itemPath != null && !Directory.Exists(itemPath))
                                {
                                    Directory.CreateDirectory(itemPath);
                                }
                                // Download item
                                using (var client = new WebClient())
                                {
                                    // Do not download BluStation.dll from blu_sprint cookbook
                                    // This file should already exist in the root, either by Bootstrap or Chef-Client run
                                    string itemFileName = Path.GetFileName(item["path"]);
                                    if (itemFileName != null && itemFileName.ToUpper() == "BLUSTATION.DLL") continue;
          
                                    // Download item
                                    client.DownloadFile(item["url"], cookbookPath + "\\" + item["path"]);
                                    string itemFolder = Path.GetDirectoryName(item["path"]);
                                    string itemPathWindowsFormat = item["path"];
                                    itemPathWindowsFormat = cookbookPath + "\\" + itemPathWindowsFormat.Replace("/", "\\");
      
                                    // Add item to resource list
                                    if (itemFolder != null && itemFolder.ToUpper() == "RESOURCES")
                                    {
                                        ResourceList.Add(itemPathWindowsFormat);
                                    }

                                    // Add item to attribute list
                                    if (itemFolder != null && itemFolder.ToUpper() == "ATTRIBUTES")
                                    {
                                        AttributeList.Add(itemPathWindowsFormat);
                                    }

                                    Logger.log("ok", "Received: " + cookbookName + " -> " + item["path"]);
                                }
                            }
                            catch (Exception ex)
                            {
                                rt.Result = 3;
                                rt.Data = String.Empty;
                                rt.Object = null;
                                rt.Message = "Unable to download: " + cookbookPath + "\\" + item["path"] + " Error: " + ex.Message;
                                return rt;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rt.Result = 3;
                        rt.Data = String.Empty;
                        rt.Object = null;
                        rt.Message = "Unable to cast cookbook to a list of items, Error : " + ex.Message;
                        return rt;
                    }
                }
            }

            // Add cookbook to runlist registry key
            if (addToRunlist) AddToRunList(cookbookName, cookbookPath, version, parent);

            // Recursively download dependencies
            Dictionary<string, dynamic> cookbookDeps = (Dictionary<string, dynamic>)ChefEndpoint.Get("cookbooks/" + cookbookName + "/" + version, "metadata/dependencies").Object;
            foreach (KeyValuePair<string, dynamic> dep in cookbookDeps)
            {
                string ver = new Regex("[^0-9 .]").Replace(dep.Value, "").Replace(" ", "");
                string op = new Regex("[^=~>]").Replace(dep.Value, "").Replace(" ", "");
                Download(dep.Key, ver, parent + "\\" + cookbookName, op, rewrite, onlyBlu, addToRunlist);
            }

            // Finally
            return rt;
        }

        /// <summary>
        /// Add a cookbook to the runlist registry key (HLKM\SOFTWARE\Blu\Runtime\Runlist)
        /// </summary>
        /// <param name="cookbookName">Cookbook name</param>
        /// <param name="cookbookPath">The absolute path to the downloaded cookbook</param>
        /// <param name="version">Cookbook version</param>
        /// <param name="parent">Parent cookbook for recursive addition</param>
        private static void AddToRunList(string cookbookName, string cookbookPath, string version, string parent)
        {
            RegistryHelper rh = new RegistryHelper();
            if (parent == String.Empty)
            {
                rh.SubKey = "SOFTWARE\\Blu\\Runtime\\RunList\\" + cookbookName;
            }
            else
            {
                rh.SubKey = "SOFTWARE\\Blu\\Runtime\\RunList" + parent + "\\" + cookbookName;
            }
            rh.Write("version", version);
            rh.Write("path", cookbookPath);
        }

        /// <summary>
        /// Checks if a cookbook supports blu. in metadata: supports 'blu'
        /// </summary>
        /// <param name="cookbookName">The name of the cookbook</param>
        /// <returns>Returns true if cookbook supports blu</returns>
        private static bool SupportsBlu(string cookbookName)
        {
            Dictionary<string, dynamic> platforms =
                (Dictionary<string, dynamic>)ChefEndpoint.Get("cookbooks/" + cookbookName + "/_latest", "metadata/platforms").Object;
            return platforms.ContainsKey("blu");
        }

        /// <summary>
        /// Find version of the cookbook to download, based on the cookbook version operators, e.g. >= 
        /// </summary>
        /// <param name="cookbookName">Cookbook name</param>
        /// <param name="version">Cookbook version</param>
        /// <param name="opr">Operator</param>
        /// <returns></returns>
        private static string FindVersion(string cookbookName, string version, string opr)
        {
            // If version is not specified, return "_latest" as version
            switch (version)
            {
                case "":
                    return "_latest";
                case "latest":
                    return "_latest";
                case "_latest":
                    return version;
            }

            // TODO: Make a better implementation of cookbook version requests
            string foundVersion;
            Version latestVersion =
                new Version((String)ChefEndpoint.Get("cookbooks/" + cookbookName + "/_latest", "version").Object);
            Version requestedVersion = new Version(version);

            // TODO: fix this
            if (version == requestedVersion.ToString()) foundVersion = requestedVersion.ToString();

            switch (opr)
            {
                case ">=":
                    foundVersion = latestVersion >= requestedVersion ? latestVersion.ToString() : String.Empty;
                    break;

                case "~>":
                    // returns String.Smily :)
                    foundVersion = latestVersion >= requestedVersion ? latestVersion.ToString() : String.Empty;
                    break;

                // TODO: Cover other cookbook version comparison operators here
                default:
                    foundVersion = String.Empty;
                    break;
            }
            // Can not find a version of this cookbook
            if (foundVersion == String.Empty)
            {
                throw new InvalidOperationException("Can not find cookbook " + cookbookName + " " + opr + " " + version);
            }
            return foundVersion;
        }

        /// <summary>
        /// Convert cookbook json object to a dictionary of items
        /// </summary>
        /// <param name="cookbookName">Cookbook name</param>
        /// <param name="version">Cookbook version</param>
        /// <returns>Dictionary of cookbook items</returns>
        private static Dictionary<string, dynamic> CookbookToDictionary(string cookbookName, string version)
        {
            Dictionary<string, dynamic> cookbook;
            try
            {
                cookbook = (Dictionary<string, dynamic>)ChefEndpoint.Get("cookbooks/" + cookbookName + "/" + version, "/").Object;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not convert cookbook to a dictionary of items, Error : " + ex.Message);
            }
            return cookbook;
        }
    }
}
