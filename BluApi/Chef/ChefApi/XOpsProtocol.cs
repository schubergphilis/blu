using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.OpenSsl;
using BluApi.Common;

namespace BluApi.Chef.ChefAPI
{
    public class XOpsProtocol
    {
        private readonly string _chefClient;
        private readonly Uri _chefUri;
        private readonly HttpMethod _httpMethod;
        private readonly string _body;
        private readonly string _timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        private string _signature = String.Empty;

        public XOpsProtocol(string chefClient, Uri chefUri)
            : this(chefClient, chefUri, HttpMethod.Get, String.Empty)
        {
        }

        public XOpsProtocol(string chefClient, Uri chefUri, HttpMethod httpMethod, string body)
        {
            _chefClient = chefClient;
            _chefUri = chefUri;
            _httpMethod = httpMethod;
            _body = body;
        }

        public HttpRequestMessage CreateMessage()
        {
            var httpRequestMessage = new HttpRequestMessage(_httpMethod, _chefUri);

            httpRequestMessage.Headers.Add("Accept", "application/json");
            httpRequestMessage.Headers.Add("X-Ops-Sign", "algorithm=sha1;version=1.0");
            httpRequestMessage.Headers.Add("X-Ops-UserId", _chefClient);
            httpRequestMessage.Headers.Add("X-Ops-Timestamp", _timestamp);
            httpRequestMessage.Headers.Add("X-Ops-Content-Hash", _body.ToBase64EncodedSha1String());
            httpRequestMessage.Headers.Add("Host", String.Format("{0}:{1}", _chefUri.Host, _chefUri.Port));
            httpRequestMessage.Headers.Add("X-Chef-Version", "11.4.0");

            if (_httpMethod != HttpMethod.Get && _httpMethod != HttpMethod.Delete)
            {
                httpRequestMessage.Content = new ByteArrayContent(Encoding.UTF8.GetBytes(_body));
                httpRequestMessage.Content.Headers.Add("Content-Type", "application/json");
            }

            var i = 1;
            foreach (var line in _signature.Split(60))
            {
                httpRequestMessage.Headers.Add(String.Format("X-Ops-Authorization-{0}", i++), line);
            }

            return httpRequestMessage;
        }

        public void SignMessage(string privateKey)
        {
            string canonicalHeader =
                String.Format(
                    "Method:{0}\nHashed Path:{1}\nX-Ops-Content-Hash:{4}\nX-Ops-Timestamp:{3}\nX-Ops-UserId:{2}",
                    _httpMethod,
                    _chefUri.AbsolutePath.ToBase64EncodedSha1String(),
                    _chefClient,
                    _timestamp,
                    _body.ToBase64EncodedSha1String());

            byte[] input = Encoding.UTF8.GetBytes(canonicalHeader);

            TextReader pemStream;

            if (privateKey.Contains("UNSET") && ChefConfig.ValidationKey != "UNSET")
            {
                ChefConfig.ValidationKey = ChefConfig.ValidationKey.Replace("-----BEGIN RSA PRIVATE KEY-----", "-----BEGIN RSA PRIVATE KEY-----\n");
                ChefConfig.ValidationKey = ChefConfig.ValidationKey.Replace("-----END RSA PRIVATE KEY-----", "\n-----END RSA PRIVATE KEY-----");
                pemStream = new StringReader(ChefConfig.ValidationKey);
            }
            else
            {
                pemStream = File.OpenText(privateKey);
            }
            
            var pemReader = new PemReader(pemStream);

            AsymmetricKeyParameter key = ((AsymmetricCipherKeyPair)pemReader.ReadObject()).Private;

            ISigner signer = new RsaDigestSigner(new NullDigest());
            signer.Init(true, key);
            signer.BlockUpdate(input, 0, input.Length);

            _signature = Convert.ToBase64String(signer.GenerateSignature());
        }
    }
}
