using System;
using System.IO;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        /// <summary>
        /// Extracts blu script (script between [Blu] tags) from a resource
        /// </summary>
        /// <param name="scriptPath">Absolute path to recipe file</param>
        /// <returns>Multiple type rt. rt.Result 0 or 3 for success and failure. rd.Data is the extracted script.</returns>
        public Function ExtactBluScript(string scriptPath)
        {
            ReturnType rt = new ReturnType { Result = 0 };
            string[] lines = File.ReadAllLines(scriptPath);
            int begin = 0;
            int end = 0;
            string data = String.Empty;

            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("=begin") && lines[i + 1].StartsWith("[Blu]")) begin = i + 2;
                    if (lines[i].StartsWith("=end") && lines[i - 1].StartsWith("[Blu]")) end = i - 2;
                }
                for (int i = begin; i < end; i++)
                {
                    data += lines[i] + Environment.NewLine;
                }
                
                data = data.Trim();

                if (string.IsNullOrEmpty(data))
                {
                    rt.Result = 3;
                    rt.Message = "Can not find blu script in the recipe " + scriptPath + ". [Blu] tags are missing in the recipe.";
                    return rt;
                }
                rt.Result = 0;
                rt.Data = data;
                return rt;
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Message = ex.Message;
                return rt;
            }
        }
    }
}
