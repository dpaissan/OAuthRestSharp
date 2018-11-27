using System;
using RestSharp;

namespace OAuthRestSharp
{
    public class OAuthCredentials : IOAuthCredentials
    {
        public IRestClient Client { get; private set; }

        public string ClientId { get; private set; }

        public Func<string> GetToken { get; private set; }

        public string RequestUrl { get; private set; }

        public string Scope { get; private set; }

        public string Secret { get; private set; }

        public Action<string> SetToken { get; private set; }

        public IRestResponse OAuthResponse { get; set; }

        public OAuthCredentials(IRestClient oAuthClient, string relativeUrl, string clientId, string secret, string scope, Func<string> getToken = null, Action<string> setToken = null)
        {
            if (oAuthClient == null)
                throw new ArgumentException("The client is null.");
            Client = oAuthClient;
            RequestUrl = relativeUrl;
            ClientId = clientId;
            Secret = secret;
            Scope = scope;
            GetToken = getToken;
            SetToken = setToken;
        }
    }
}
