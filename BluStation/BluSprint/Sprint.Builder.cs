using System;
using BluApi.Common;
using BluApi.Chef.ChefAPI;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        public Function Build()
        {
            ReturnType rt;

            // Check Connection to Chef API
            Logger.log("info", "Connecting to Chef API...");
            rt = ChefEndpoint.Get("nodes/" + ChefConfig.NodeName, "/");
            if (rt.Result != 0) return rt;
            Logger.log("ok", "Connected!");

            // Disable apilog after succesfull connection (to keep the log clean and readable)
            ChefConfig.ApiLog = false;

            // Clean up
            // TODO: Do some magic with MD5 instead of simply deleting the folder content
            Logger.log("info", "Preparing Runtime folder.");
            SprintData.RunlistPath.EmptyFolder();
            SprintData.SprintRunList.Clear();
            SprintData.ResourceFileList.Clear();
            SprintData.AttributeFileList.Clear();

            // Build Runlist (defined in Sprint.Runlist)
            BuildRunList();
            if (rt.Result != 0) return rt;

            // Import Header (defined in Sprint.Header)
            ImportHeader();
            
            // Define sprint cookbook depnding of the Mode paramter
            rt = DefineSprintCookbook(Mode);
            if (rt.Result != 0) return rt;
            SprintData.SprintPath = rt.Data;

            // Import constant variables (defined in Sprint.Constants)
            ImportConstants();

            // Import Attributes (defined in Sprint.Attributes)
            ImportAttributes();

            // Import instant unit tests
            Logger.log("info", "Reading Test script: blu_sprint::test.blu.rb");
            rt = ImportTest();
            if (rt.Result != 0) return rt;

            // Collect Resource Files (defined in SprintData.ResourceFileList)
            rt = CollectResources();
            if (rt.Result != 0) return rt;

            // Collect Attribute Files (defined in SprintData.AttributeFileList)
            rt = CollectAttributes();
            if (rt.Result != 0) return rt;

            // Collect Recipes
            rt = CollectRecipes();
            if (rt.Result != 0) return rt;

            // Build Ruby Stack
            RubyStack = AttributesStack + Environment.NewLine + RecipeStack;
            
            // Compile Sprint
            Compile();

            // Write Ruby stack to RubyStack.rb file
            System.IO.File.WriteAllText(SprintData.RubyStack, RubyStack);

            // Write transpiled Resources.psm1 to file
            System.IO.File.WriteAllText(SprintData.CompiledSprint, SprintPs1Content);

            // Write transpiled Sprint.ps1 to file
            System.IO.File.WriteAllText(SprintData.CompiledResources, ResourcesPs1Content);
            
            return rt;
        }
    }
}
