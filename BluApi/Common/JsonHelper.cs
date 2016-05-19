using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluApi.Common
{
    public static class JsonHelper
    {
        /// <summary>
        /// Serialize a Dictionary to Json 
        /// </summary>
        /// <param name="dictionary">Input as Dictionary</param>
        /// <returns>Json as string</returns>
        public static string Serialize(Dictionary<string, dynamic> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        }
        
        /// <summary>
        /// Deserialize json object to C# object
        /// </summary>
        /// <param name="json">json as string</param>
        /// <returns></returns>
        public static object Deserialize(string json)
        {
            return ToObject(JToken.Parse(json));
        }

        
        /// <summary>
        /// Convert json string to C# object
        /// </summary>
        /// <param name="json">json as string</param>
        /// <returns></returns>
        public static object ToObject(string json)
        {
            JToken jsonToken = JToken.Parse(json);
            return ToObject(jsonToken);
        }
        
        /// <summary>
        /// Convert Json.Net Jtoken to C# object
        /// Depending on the token type it returns object as Dictionary, List or String
        /// </summary>
        /// <param name="token">Json.Net Jtoken as token</param>
        /// <returns></returns>
        public static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));
                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}
