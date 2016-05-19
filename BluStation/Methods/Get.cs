using System.IO;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace Blu
{
    public static partial class Method
    {
        /// <summary>
        /// ChefApi GET method
        /// </summary>
        /// <param name="select">Select endpoint on Chef server</param>
        /// <param name="path">Search path</param>
        /// <returns>Chef server reply as dictionary, array or string</returns>
        public static object Get(string select, string path)
        {
            ReturnType rt = ChefEndpoint.Get(select, path);
            if (rt.Result == 0)
            {
                return rt.Object;
            }
            else
            {
                throw new System.InvalidOperationException(rt.Message);
            }
        }

        /// <summary>
        /// Casts node_object.json to a dictionary for PowerShell
        /// </summary>
        /// <returns>node object as dictionary</returns>
        public static object GetNodeObject()
        {
            string nodePath = ChefConfig.ClientPath + @"\blu\runtime\node_object.json";
            if (File.Exists(nodePath))
            {
                string json = File.ReadAllText(nodePath);
                object obj = JsonHelper.ToObject(json);
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    throw new System.InvalidOperationException("Unable to create Node Object.");
                }
            }
            else
            {
                throw new System.InvalidOperationException("Unable to create Node Object.");
            }
        }
    }
}
