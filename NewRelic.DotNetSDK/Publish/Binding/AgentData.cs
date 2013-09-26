using System.Collections.Generic;

namespace NewRelic.DotNetSDK.Publish.Binding
{
    public class AgentData
    {
        //// ----------------------------------------------------------------------------------------------------------

        public string Host { get; set; }

        public string Version { get; set; }

        public int Pid { get; set; }

        //// ----------------------------------------------------------------------------------------------------------
		 
        public Dictionary<string, object> Serialize()
        {
            var output = new Dictionary<string, object> { { "host", Host }, { "version", Version }, { "pid", Pid } };

            return output;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}