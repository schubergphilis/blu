using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BluApi.Chef.ChefAPI;
using BluApi.Chef.ChefResources;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public Function BuildRunList()
        {
            ReturnType rt = new ReturnType();
            Logger.log("info", "Building run list...");
            RegistryHelper rh = new RegistryHelper {SubKey = "SOFTWARE\\Blu\\Runtime\\RunList\\"};
            rh.DeleteSubKeyTree();
            
            List<object> draftRunlist = (List<object>)ChefEndpoint.Get("nodes/" + ChefConfig.NodeName, "run_list").Object;
           
            // TODO: handle roles in the runlist
            // Only extract recipe[] runlist for now, role[] is not handled yet
            foreach (string item in draftRunlist)
            {
                Cookbook cookbook = new Cookbook();
                string recipe = item.StringBetween("recipe[", "]");
                string[] runlistItem = Regex.Split(recipe, @"::");
                string cookbookName = runlistItem[0];

                // Add runlist item to sprint runlist
                string recipeName = runlistItem.Count() < 2 ? "default" : runlistItem[1];
                string sprintRunListItem = SprintData.RunlistPath + "\\" + cookbookName + "\\recipes\\" + recipeName + ".rb";
                SprintData.SprintRunList.Add(sprintRunListItem);

                // Download cookbooks and extract resource/attribute list
                rt = cookbook.Download(cookbookName, String.Empty, String.Empty, String.Empty, true, true, true);
                SprintData.ResourceFileList.AddRange(cookbook.ResourceList);
                SprintData.AttributeFileList.AddRange(cookbook.AttributeList);
            }
            return rt;
        }
    }
}
