using System.Collections.Generic;
using Newtonsoft.Json;
using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Chef.ChefResources
{
    public class Client
    {
        public Function Add(string clientName, bool force)
        {
            ReturnType rt = Add(clientName);

            switch (rt.Result)
            {
                case 0:
                    return rt;
                case 3:
                    if (force)
                    {
                        Logger.log("warn", "Client already exists and -Force key is provided. Trying to delete and recreate client.");
                        rt = Delete(clientName);
                        if (rt.Result == 0) rt = Add(clientName);
                        return rt;
                    }
                    break;
            }
            return rt;
        }

        public Function Add(string clientName)
        {
            ReturnType rt = new ReturnType();
           
            Logger.log("info", "Attempting to create client: " + clientName);

            ChefRequest cr = new ChefRequest();

            Dictionary<string, string> dictClient = new Dictionary<string, string>()
            {
                { "name", clientName },
                { "admin", "false" }
            };

            string json = JsonConvert.SerializeObject(dictClient, Formatting.Indented);
            string response = cr.Post(ChefConfig.Validator, "clients", json);
            if (response.Contains("409") && response.Contains("Conflict"))
            {
                rt.Result = 3;
                rt.Message = "Client already exists.";
                return rt;
            }
            else if (response.Contains("BEGIN RSA PRIVATE KEY"))
            {
                KeyHelper kh = new KeyHelper();
                rt = kh.Format(response);
                return rt;
            }
            else
            {
                rt.Result = 4;
                rt.Message = "Error creating Client, API response: " + response;
                return rt;
            }
        }


        public Function Delete(string clientName)
        {
            ReturnType rt = new ReturnType();
            
            Logger.log("info", "Attempting to delete client: " + clientName);

            ChefRequest cr = new ChefRequest();
            string response = cr.Delete(ChefConfig.Validator, "clients/" + clientName);

            if (response.Contains("Response status code does not indicate success"))
            {
                rt.Result = 4;
                rt.Data = null;
                rt.Object = null;
                rt.Message = "Unable to delete client.";
            }
            else
            {
                rt.Result = 0;
                rt.Data = response;
                rt.Object = null;
                rt.Message = "Client: " + clientName + " is deleted.";
            }
            return rt;
        }
    }
}
