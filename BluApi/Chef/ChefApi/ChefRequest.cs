using System;
using System.Net.Http;
using BluApi.Common;

namespace BluApi.Chef.ChefAPI
{
    public class ChefRequest
    {
        public string Get(string restClient, string restNode)
        {
            try
            {
                return RequestData("GET", restClient, restNode, "");
            }
            catch (Exception ex)
            {
                return "Unable to GET data, Error: " + ex.Message;
            }
        }

        public string Post(string restClient, string restNode, string body)
        {
            try
            {
                return RequestData("POST", restClient, restNode, body);
            }
            catch (Exception ex)
            {
                return "Unable to POST data, Error: " + ex.Message;
            }
        }

        public string Put(string restClient, string restNode, string body)
        {
            try
            {
                return RequestData("PUT", restClient, restNode, body);
            }
            catch (Exception ex)
            {
                return "Unable to PUT data, Error: " + ex.Message;
            }
        }

        public string Delete(string restClient, string restNode)
        {
            try
            {
                return RequestData("DELETE", restClient, restNode, String.Empty);
            }
            catch (Exception ex)
            {
                return "Unable to DELETE " + restNode + " Error: " + ex.Message;
            }
        }

        private static string RequestData(string method, string client, string resource, string body)
        {
            Uri organizationUri = ChefConfig.OrganizationUri;
            string pemPath = ChefConfig.ClientPem;
            var chefApi = new ChefApi(organizationUri);

            switch (method)
            {
                // Method GET
                case "GET":
                    var getUri = new Uri(organizationUri + "/" + resource);
                    var xOpsGet = new XOpsProtocol(client, getUri);

                    Logger.log("api", "Method : GET");
                    Logger.log("api", "Client : " + client);
                    Logger.log("api", "organizationUri : " + organizationUri);
                    Logger.log("api", "resource : " + resource);
                    Logger.log("api", "getUri : " + getUri);
                    Logger.log("api", "XOpsProtocol(" + client + ", " + getUri + ");");

                    try
                    {
                        xOpsGet.SignMessage(pemPath);
                        return chefApi.SendRest(xOpsGet);
                    }
                    catch (Exception ex)
                    {
                        Logger.log("api", "API request GET Error " + getUri + ": " + ex.Message);
                        return "API request GET Error " + getUri + ": " + ex.Message;
                    }

                // Method POST
                case "POST":
                    HttpMethod postMethod = new HttpMethod("POST");
                    var postUri = new Uri(organizationUri + "/" + resource);
                    var xOpsPost = new XOpsProtocol(client, postUri, postMethod, body);

                    Logger.log("api", "Method : " + postMethod);
                    Logger.log("api", "Client : " + client);
                    Logger.log("api", "organizationUri : " + organizationUri);
                    Logger.log("api", "resource : " + resource);
                    Logger.log("api", "postUri : " + postUri);
                    Logger.log("api", "XOpsProtocol(" + client + ", " + postUri + ", " + postMethod + ", <body>)");

                    try
                    {
                        xOpsPost.SignMessage(pemPath);
                        return chefApi.SendRest(xOpsPost);
                    }
                    catch (Exception ex)
                    {
                        Logger.log("api", "API request POST Error " + postUri + ": " + ex.Message);
                        return "API request POST Error " + postUri + ": " + ex.Message;
                    }

                // Method PUT
                case "PUT":
                    HttpMethod putMethod = new HttpMethod("PUT");
                    var putUri = new Uri(organizationUri + "/" + resource);
                    var xOpsPut = new XOpsProtocol(client, putUri, putMethod, body);

                    Logger.log("api", "Method : " + putMethod);
                    Logger.log("api", "Client : " + client);
                    Logger.log("api", "organizationUri : " + organizationUri);
                    Logger.log("api", "resource : " + resource);
                    Logger.log("api", "putUri : " + putUri);
                    Logger.log("api", "XOpsProtocol(" + client + ", " + putUri + ", " + putMethod + ", <body>)");

                    try
                    {
                        xOpsPut.SignMessage(pemPath);
                        return chefApi.SendRest(xOpsPut); 
                    }
                    catch (Exception ex)
                    {
                        Logger.log("api", "API request PUT Error " + putUri + ": " + ex.Message);
                        return "API request PUT Error " + putUri + ": " + ex.Message;
                    }

                // Method DELETE
                case "DELETE":
                    HttpMethod deleteMethod = new HttpMethod("DELETE");
                    var deleteUri = new Uri(organizationUri + "/" + resource);
                    var xOpsDelete = new XOpsProtocol(client, deleteUri, deleteMethod, String.Empty);

                    Logger.log("api", "Method : DELETE");
                    Logger.log("api", "Client : " + client);
                    Logger.log("api", "organizationUri : " + organizationUri);
                    Logger.log("api", "resource : " + resource);
                    Logger.log("api", "deleteUri : " + deleteUri);
                    Logger.log("api", "XOpsProtocol(" + client + ", " + deleteUri + ");");

                    try
                    {
                        xOpsDelete.SignMessage(pemPath);
                        return chefApi.SendRest(xOpsDelete);
                    }
                    catch (Exception ex)
                    {
                        Logger.log("api", "API request DELETE Error " + deleteUri + ": " + ex.Message);
                        return "API request DELETE Error " + deleteUri + ": " + ex.Message;
                    }

                default:
                    Logger.log("api", "Method " + method + " is not recognized.");
                    return "Method " + method + " is not supported.";
            }
        }
    }
}
