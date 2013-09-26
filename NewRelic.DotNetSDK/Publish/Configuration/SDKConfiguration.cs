using System;
using System.Configuration;

using NewRelic.DotNetSDK.Publish.Binding;

namespace NewRelic.DotNetSDK.Publish.Configuration
{
    public class SDKConfiguration
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        private const string DefaultLicenseKey = "YOUR_LICENSE_KEY_HERE";
        private const int DefaultPollInterval = 60;

        private const string ConfigGroupName = "newRelicGroup";
        private const string ConfigSectionName = "newRelicSettings";

        private string licenseKey;
        private string serviceUri;
        private bool sslHostVerification = true;
        private int pollInterval;

        //// ----------------------------------------------------------------------------------------------------------
		
        public SDKConfiguration()
        {
            LoadConfigurationFromApplicationConfiguration(ConfigGroupName, ConfigSectionName);
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        private void LoadConfigurationFromApplicationConfiguration(string configGroupName, string configSectionName)
        {
            try
            {
                var sectionName = string.Format("{0}/{1}", configGroupName, configSectionName);

                var configSection = (NewRelicConfigurationSection)ConfigurationManager.GetSection(sectionName);

                licenseKey = configSection.LicenseKey;
                serviceUri = configSection.ServiceUri;
                sslHostVerification = configSection.SslHostVerification;
                pollInterval = configSection.PollIntervalSeconds;
            }
            catch (Exception ex)
            {
                var message = string.Format("Error reading configuration: {0}", ex.Message);
                Context.GetLogger().Fatal(message);
                throw new AgentConfigurationException(message, ex);
            }
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        public int GetPollInterval()
        {
            return pollInterval == 0 ? DefaultPollInterval : pollInterval;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public string GetServiceUri()
        {
            return serviceUri;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public string GetLicenseKey()
        {
            return licenseKey ?? DefaultLicenseKey;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public bool IsSslHostVerificationEnabled()
        {
            return sslHostVerification;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}