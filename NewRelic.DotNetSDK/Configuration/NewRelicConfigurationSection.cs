using System.Configuration;

namespace NewRelic.DotNetSDK.Configuration
{
    public class NewRelicConfigurationSection : ConfigurationSection
    {
        //// ----------------------------------------------------------------------------------------------------------

        [ConfigurationProperty("licenseKey", DefaultValue = "YOUR_LICENSE_KEY_HERE", IsRequired = true)]
        public string LicenseKey
        {
            get { return this["licenseKey"] as string; }
            set { this["licenseKey"] = value; }
        }

        //// ----------------------------------------------------------------------------------------------------------

        [ConfigurationProperty("serviceUri", DefaultValue = "https://platform-api.newrelic.com/platform/v1/metrics", IsRequired = true)]
        public string ServiceUri
        {
            get { return this["serviceUri"] as string; }
            set { this["serviceUri"] = value; }
        }

        //// ----------------------------------------------------------------------------------------------------------

        [ConfigurationProperty("sslHostVerification", DefaultValue = true, IsRequired = true)]
        public bool SslHostVerification
        {
            get { return (bool)this["sslHostVerification"]; }
            set { this["serviceUri"] = value; }
        }

        //// ----------------------------------------------------------------------------------------------------------

        [ConfigurationProperty("pollIntervalSeconds", DefaultValue = 60, IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 300, MinValue = 45)]
        public int PollIntervalSeconds
        {
            get { return (int)this["pollIntervalSeconds"]; }
            set { this["pollIntervalSeconds"] = value; }
        }

        //// ----------------------------------------------------------------------------------------------------------	 
    }
}