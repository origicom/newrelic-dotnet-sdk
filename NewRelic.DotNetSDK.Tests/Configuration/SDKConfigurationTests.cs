using Microsoft.VisualStudio.TestTools.UnitTesting;

using NewRelic.DotNetSDK.Configuration;

namespace NewRelic.DotNetSDK.Tests.Configuration
{
    [TestClass]
    public class SDKConfigurationTests
    {
        //// ----------------------------------------------------------------------------------------------------------

        private const string TestLicenseKey = "TEST_LICENSE_KEY";
        private const string TestServiceUri = "https://test_service_uri";
        private const int TestPollIntervalSeconds = 90;
        private const bool TestUseSslHostVerification = false;
 
        //// ----------------------------------------------------------------------------------------------------------
		 
        [TestMethod]
        public void Constructor_ValidConfig_ExpectSettingsFromConfigFile()
        {
            /************ Arrange ************/
            SDKConfiguration configuration;

            /************ Act ****************/
            configuration = new SDKConfiguration();

            /************ Assert *************/
            Assert.AreEqual(TestLicenseKey, configuration.GetLicenseKey());
            Assert.AreEqual(TestServiceUri, configuration.GetServiceUri());
            Assert.AreEqual(TestPollIntervalSeconds, configuration.GetPollInterval());
            Assert.AreEqual(TestUseSslHostVerification, configuration.IsSslHostVerificationEnabled());
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}