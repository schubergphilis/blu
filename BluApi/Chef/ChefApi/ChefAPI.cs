using System;
using System.Net.Http;

namespace BluApi.Chef.ChefAPI
{
    class ChefApi
    {
        /// <summary>
        /// Readonly System.Uri refereing to Chef server Uri
        /// </summary>
        private readonly Uri _chefServer;
        /// <summary>
        /// Chef server Uri
        /// </summary>
        /// <param name="chefServer"></param>
        public ChefApi(Uri chefServer)
        {
            _chefServer = chefServer;
        }
        /// <summary>
        /// Sends a REST API request to Chef server
        /// </summary>
        /// <param name="xOpsProtocol">An instance of the X-Ops Protocol</param>
        /// <returns>result of the API request as async string</returns>
        public string SendRest(XOpsProtocol xOpsProtocol)
        {
            using (var restClient = new HttpClient())
            {
                restClient.BaseAddress = _chefServer;
                var payload = xOpsProtocol.CreateMessage();
                var result = restClient.SendAsync(payload).Result;
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
