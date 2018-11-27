using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace OAuthRestSharp
{
    public static class RestSharpOauthExtensions
    {
        public static IRestResponse Execute(this IRestClient client, IRestRequest request, IOAuthCredentials credentials)
        {
            return Execute(client, (c,r) => c.Execute(r), request, credentials);
        }

        public static IRestResponse<T> Execute<T>(this IRestClient client, IRestRequest request, IOAuthCredentials credentials) where T : new()
        {
            return (IRestResponse<T>)Execute(client, (c, r) => c.Execute<T>(r), request, credentials);
        }
        
        private static IRestResponse Execute(IRestClient client, Func<IRestClient, IRestRequest, IRestResponse> send, IRestRequest request, IOAuthCredentials credentials)
        {
            IRestResponse resp;
            if (credentials.GetToken != null)
                client.Authenticator = new JwtAuthenticator(credentials.GetToken());
            resp = send(client, request);

            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var oAuthRequest = new RestRequest(credentials.RequestUrl, Method.POST);

                // Headers
                oAuthRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Accept", "application/json");

                // Parameters
                oAuthRequest.AddParameter("grant_type", "client_credentials");
                oAuthRequest.AddParameter("client_id", credentials.ClientId);
                oAuthRequest.AddParameter("client_secret", credentials.Secret);
                oAuthRequest.AddParameter("scope", credentials.Scope);

                credentials.OAuthResponse = credentials.Client.Execute(oAuthRequest);

                // Parsing token
                var jsSerializer = new JavaScriptSerializer();
                Dictionary<string, object> dict = (Dictionary<string, object>)jsSerializer.DeserializeObject(credentials.OAuthResponse.Content);
                
                string token = dict?["access_token"]?.ToString() ?? "";

                // Send with the newly retrieved token
                client.Authenticator = new JwtAuthenticator(token);
                resp = send(client, request);

                // Store the token
                if ((int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300)
                    credentials.SetToken?.Invoke(token);
            }

            return resp;
        }
    }
}
