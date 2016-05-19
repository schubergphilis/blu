using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public Function CollectRecipes()
        {
            ReturnType rt = new ReturnType {Result = 0};
            Logger.log("info", "Compiling recipes in runlist...");
            foreach (string sprintRunlistItem in SprintData.SprintRunList)
            {
                // Ignore blu_sprint cookbook when compiling recipes
                if (sprintRunlistItem.Contains("blu_sprint")) continue;
                
                // Add runlist item to recipes stack
                if (File.Exists(sprintRunlistItem))
                {
                    RecipeStack += Environment.NewLine;
                    RecipeStack += "#---------------------------------------------------------------" + Environment.NewLine;
                    RecipeStack += "# " + sprintRunlistItem + Environment.NewLine;
                    RecipeStack += "#---------------------------------------------------------------" + Environment.NewLine;
                    RecipeStack += Environment.NewLine;
                    
                    string[] lines = File.ReadAllLines(sprintRunlistItem);
                    foreach (string line in lines)
                    {
                        RecipeStack += line.Trim() + Environment.NewLine;
                    }
                }
                else
                {
                    rt.Result = 3;
                    rt.Message = "Runlist item does not exist: " + sprintRunlistItem;
                    return rt;
                }
            }
            return rt;
        }
    }
}
