using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OAuthRestSharp.UnitTests
{
    [TestClass]
    public class OAuthCredentialsTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void OAuthCredentials_OAuthClient_Null()
        {
            var cred = new OAuthCredentials(null, "C", "D", "E", "F");
        }

        [TestMethod]
        public void OAuthCredentials_OAuthClient_Valid()
        {
            var cred = new OAuthCredentials(new MockRestClient(null, null), "C", "D", "E", "F");
        }
    }
}
