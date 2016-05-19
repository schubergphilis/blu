using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Chef.ChefResources
{
    public class Node
    {
        public Function Add(string nodeName, string environment, List<string> runlist, List<string> roles, bool force)
        {
            ReturnType rt = Add(nodeName, environment, runlist, roles);
            switch (rt.Result)
            {
                case 0:
                    return rt;

                case 3:
                    if (force)
                    {
                        Logger.log("warn", "Node already exists and -Force key is provided. Trying to delete and recreate node.");
                        rt = Delete(nodeName);
                        if (rt.Result == 0)
                        {
                            rt = Add(nodeName, environment, runlist, roles);
                        }
                        return rt;
                    }
                    break;
            }
            return rt;
        }

        public Function Add(string nodeName, string environment, List<string> runlist, List<string> roles)
        {
            ReturnType rt = new ReturnType();
            
            Logger.log("info", "Attempting to create node: " + nodeName);

            ChefRequest cr = new ChefRequest();
            
            // Convert runlist[] and roles[] to List run_list
            List<string> runList = new List<string>();
            
            foreach (string run in runlist)
            {
                runList.Add("recipe[" + run + "]");
            }
            foreach (string role in roles)
            {
                runList.Add("role[" + role + "]");
            }

            if (string.IsNullOrEmpty(environment)) environment = "_default";

            Dictionary<string, dynamic> dictNode = new Dictionary<string, dynamic>()
            {
                { "name", nodeName },
                { "chef_type", "node" },
                { "json_class", "Chef::Node"},
                // { "attributes", dict_node_attributes},
                { "chef_environment", environment }
            };

            if (runList.Count > 0) dictNode.Add("run_list", runList);
            
            string json = JsonConvert.SerializeObject(dictNode, Formatting.Indented);

            Logger.log("info", "Attempting to create Chef Node by POST the following json:");
            Logger.log("data", json);

            string response = cr.Post(ChefConfig.Validator, "nodes", json);

            if (response.Contains("409") && response.Contains("Conflict"))
            {
                rt.Result = 3;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Node already exists.";
                return rt;
            }
            if (response.Contains("uri"))
            {
                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Chef Node: " + nodeName + " is created succesfully.";
                return rt;
            }
            rt.Result = 4;
            rt.Data = String.Empty;
            rt.Object = null;
            rt.Message = "Error creating Node, API response: " + response;
            return rt;
        }


        public Function Delete(string nodeName)
        {
            ReturnType rt = new ReturnType();
            
            Logger.log("info", "Attempting to delete node: " + nodeName);

            ChefRequest cr = new ChefRequest();
            string response = cr.Delete(ChefConfig.Validator, "nodes/" + nodeName);

            if (response.Contains("Response status code does not indicate success"))
            {
                rt.Result = 4;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Unable to delete node.";
            }
            else
            {
                rt.Result = 0;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Node: " + nodeName + " is deleted.";
            }
            return rt;
        }

        
        public Object GetRunList()
        {
            Object runList = ChefEndpoint.Get("nodes/" + ChefConfig.NodeName, "run_list").Object;
            return runList;
        }
    }
}
