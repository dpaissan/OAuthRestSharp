using RestSharp;
using System;

namespace OAuthRestSharp
{
    public interface IOAuthCredentials
    {
        IRestClient Client { get; }
        string RequestUrl { get; }
        string ClientId { get; }
        string Secret { get; }
        string Scope { get; }
        Func<string> GetToken { get; }
        Action<string> SetToken { get; }
        IRestResponse OAuthResponse { get; set; }
    }
}
