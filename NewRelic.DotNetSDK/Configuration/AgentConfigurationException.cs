using System;

namespace NewRelic.DotNetSDK.Configuration
{
    public class AgentConfigurationException : Exception
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        public AgentConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //// ----------------------------------------------------------------------------------------------------------

        public AgentConfigurationException(string message)
            : base(message)
        {
        }

        //// ----------------------------------------------------------------------------------------------------------
    }
}