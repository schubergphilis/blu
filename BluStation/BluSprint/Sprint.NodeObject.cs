using System;
using System.IO;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluStation.BluSprint
{
    public partial class Sprint
    {
        /// <summary>
        /// Saves node object to node_objec.json (ChefConfig.NodeObject)
        /// </summary>
        /// <param name="json">node object as json (string)</param>
        /// <returns>Multiple type rt. rt.Result 0 is success, 3 if failed</returns>
        public Function SaveNodeObject(string json)
        {
            ReturnType rt = new ReturnType { Result = 0 };
            if (string.IsNullOrEmpty(json))
            {
                rt.Result = 3;
                rt.Message = "Node object (json) string can not be empty.";
                return rt;
            }

            try
            {
                File.WriteAllText(json + @"\node_object.json", SprintData.NodeObject);
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Message = ex.Message;
                return rt;
            }
            return rt;
        }
    }
}
