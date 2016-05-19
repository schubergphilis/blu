using System.IO;
using BluApi.Chef.ChefAPI;
using BluApi.Chef.ChefResources;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        /// <summary>
        /// Checks if a development version of blu_sprint cookbook exists
        /// Determines sprintPath (absolute path to blu_sprint cookbook)
        /// </summary>
        /// <returns>Multiple type rt (sprintPath as rt.Data)</returns>
        public Function DefineSprintCookbook(string mode)
        {
            ReturnType rt = new ReturnType();
            string sprintPath;

            // Check if Mode is dev and development version of blu_sprint exists
            if (mode.ToUpper() == "DEV" && ChefConfig.DevPath != "UNSET" && Directory.Exists(ChefConfig.DevPath + "\\blu_sprint"))
            {
                Logger.log("info", "Development version of blu_sprint exist. Starting local Sprint for development.");
                sprintPath = ChefConfig.DevPath + "\\blu_sprint";
                rt.Result = 0;
                rt.Data = sprintPath;
                return rt;
            }
            else
            {
                // Development version of blu_sprint does not exist, use the live version
                sprintPath = SprintData.RunlistPath + "\\blu_sprint";
                rt.Result = 0;
                rt.Data = sprintPath;
                return rt;
            }
        }
    }
}
