using System;
using ReturnType = BluApi.Common.Function;

namespace BluApi.Common
{
    /// <summary>
    /// RSA key helper
    /// </summary>
    public class KeyHelper
    {
        /// <summary>
        /// Formats the RSA key, to be used by BluApi Chef API 
        /// </summary>
        /// <param name="key">RSA key as sting</param>
        /// <returns>RT multiple type: see BluApi.Common.Function</returns>
        public Function Format(string key)
        {
            ReturnType rt = new ReturnType();
            if (key.Contains("-----BEGIN RSA PRIVATE KEY-----") && key.Contains("-----END RSA PRIVATE KEY-----"))
            {
                key = key.Replace(Environment.NewLine, String.Empty);
                key = key.Replace(@"\n", String.Empty);
                key = key.Replace(@"\r", String.Empty);
                key = key.Replace(@"\t", String.Empty);

                int beginRsa = key.IndexOf("-----BEGIN RSA PRIVATE KEY-----", StringComparison.Ordinal) + 31;
                int endRsa = key.IndexOf("-----END RSA PRIVATE KEY-----", StringComparison.Ordinal);
                string rsaKey = key.Substring(beginRsa, endRsa - beginRsa);
                int stringLength = rsaKey.Length;
                int chunkSize = 64;

                string formattedRsaKey = "-----BEGIN RSA PRIVATE KEY-----" + Environment.NewLine;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    formattedRsaKey += rsaKey.Substring(i, chunkSize) + Environment.NewLine;
                }
                formattedRsaKey += "-----END RSA PRIVATE KEY-----";
                
                rt.Result = 0;
                rt.Data = formattedRsaKey;
                rt.Object = null;
                rt.Message = String.Empty;
            }
            else
            {
                rt.Result = 3;
                rt.Data = String.Empty;
                rt.Object = null;
                rt.Message = "Failed to format RSA key. Cannot find BEGIN and END of RSA Key.";
            }
            return rt;
        }
    }
}
