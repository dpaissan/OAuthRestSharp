using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace OAuthRestSharp.UnitTests
{
    [TestClass]
    public class RestSharpOauthExtensionsTest
    {
        [TestMethod]
        public void RestSharpOauthExtensionsTest_ZeroStatusCode()
        {
            // Assembly
            int howmany = 0;
            Func<IRestRequest, IRestResponse> exec = r =>
            {
                howmany++;
                return new MockRestResponse("", 0);
            };
            IRestClient client = new MockRestClient(new Uri("B://B"), exec);

            var req = new RestRequest("A", Method.GET);
            var cred = new OAuthCredentials(new MockRestClient(new Uri("B://B"), null), "C", "D", "E", "F");

            // Apply
            var resp = client.Execute(req, cred);

            // Assert
            Assert.AreEqual(1, howmany);
        }

        [TestMethod]
        public void RestSharpOauthExtensionsTest_OkStatusCode()
        {
            // Assembly
            int howmany = 0;
            Func<IRestRequest, IRestResponse> exec = r =>
            {
                howmany++;
                return new MockRestResponse("", System.Net.HttpStatusCode.OK);
            };
            IRestClient client = new MockRestClient(new Uri("B://B"), exec);
            var req = new RestRequest("A", Method.GET);
            var cred = new OAuthCredentials(new MockRestClient(new Uri("B://B"), null), "C", "D", "E", "F");

            // Apply
            var resp = client.Execute(req, cred);

            // Assert
            Assert.AreEqual(1, howmany);
        }

        [TestMethod]
        public void RestSharpOauthExtensionsTest_UnauthorizedStatusCode()
        {
            // Assembly
            int howManyClientExecute = 0;
            int howManyAuthenticator = 0;
            int howManyOAuthExecute = 0;

            IRestClient client = new MockRestClient(new Uri("B://B"),
                r => { howManyClientExecute++; return new MockRestResponse("", System.Net.HttpStatusCode.Unauthorized); },
                (a) => { howManyAuthenticator++; });
            var req = new RestRequest("A", Method.GET);
            var cred = new OAuthCredentials(new MockRestClient(new Uri("B://B"),
                r => { howManyOAuthExecute++; return new MockRestResponse("", System.Net.HttpStatusCode.OK); }),
                "C", "D", "E", "F");

            // Apply
            var resp = client.Execute(req, cred);

            // Assert
            Assert.AreEqual(2, howManyClientExecute);
            Assert.AreEqual(1, howManyAuthenticator);
            Assert.AreEqual(1, howManyOAuthExecute);
        }

        [TestMethod]
        public void RestSharpOauthExtensionsTest_GetToken()
        {
            // Assembly
            int howManyClientExecute = 0;
            int howManyAuthenticator = 0;
            int howManyOAuthExecute = 0;
            int howManyGetToken = 0;
            int howManySetToken = 0;
            string tokenSet = "INVALID";

            IRestClient client = new MockRestClient(new Uri("B://B"),
                r => {
                    howManyClientExecute++;

                    if(howManyClientExecute == 1)
                        return new MockRestResponse("", System.Net.HttpStatusCode.Unauthorized);
                    else
                        return new MockRestResponse("", System.Net.HttpStatusCode.OK);
                },
                (a) => { howManyAuthenticator++; });
            var req = new RestRequest("A", Method.GET);
            var cred = new OAuthCredentials(
                new MockRestClient(new Uri("B://B"), r => { howManyOAuthExecute++; return new MockRestResponse("{\"access_token\": \"TOKEN2\"}", System.Net.HttpStatusCode.OK); }),
                "C", "D", "E", "F", ()=> { howManyGetToken++; return "TOKEN1"; }, (t) => { howManySetToken++; tokenSet = t; });

            // Apply
            var resp = client.Execute(req, cred);

            // Assert
            Assert.AreEqual(2, howManyClientExecute);
            Assert.AreEqual(2, howManyAuthenticator);
            Assert.AreEqual(1, howManyOAuthExecute);
            Assert.AreEqual(1, howManyGetToken);
            Assert.AreEqual(1, howManySetToken);
            Assert.AreEqual("TOKEN2", tokenSet);
        }
    }
}
