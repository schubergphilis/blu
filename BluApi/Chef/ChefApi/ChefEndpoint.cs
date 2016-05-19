using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Chef.ChefAPI
{
    public static class ChefEndpoint
    {
        public static Function Get(string endpoint, string path)
        {
            ReturnType rt = new ReturnType();
            ChefRequest cr = new ChefRequest();
            string json = cr.Get(ChefConfig.ClientName, endpoint);
            JToken jsonToken;

            // Accept empty path as /
            if (string.IsNullOrEmpty(path)) path = "/";
            
            try
            {
                jsonToken = JToken.Parse(json);
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Object = null;
                rt.Data = String.Empty;
                rt.Message = "Error deserializing Endpoint Json object: " + ex.Message + 
                    "\r\n--------" + 
                    "\r\nServer reply:\r\n" + json +
                    "\r\n--------" ;
                return rt;
            }
            
            if (path != "/")
            {
                path = path.TrimStart('/');
                List<string> pathList = path.Split('/').ToList();
                try
                {
                    foreach (string pth in pathList)
                    {
                        jsonToken = jsonToken[pth];
                    }
                    rt.Object = JsonHelper.ToObject(jsonToken);
                    rt.Data = jsonToken.ToString();
                    rt.Result = 0;
                    rt.Message = String.Empty;
                    return rt;
                }
                catch (Exception ex)
                {
                    rt.Result = 3;
                    rt.Object = null;
                    rt.Data = String.Empty;
                    rt.Message = "Error deserializing Endpoint Json object: " + ex.Message;
                    return rt;
                }
            }
            
            try
            {
                rt.Result = 0;
                rt.Object = JsonHelper.ToObject(jsonToken);
                rt.Data = json;
                rt.Message = String.Empty;
                return rt;
            }
            catch (Exception ex)
            {
                rt.Result = 3;
                rt.Object = null;
                rt.Data = String.Empty;
                rt.Message = "Error deserializing Endpoint Json object: " + ex.Message;
                return rt;
            }
        }
    }
}
