using System.Collections.Generic;
using System.Configuration;
using System.IO;

using NewRelic.DotNetSDK.Binding;
using NewRelic.DotNetSDK.Runners;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewRelic.DotNetSDK
{
    /// <summary>
    /// A factory for creating configured <see cref="Agent" />s. <see cref="AgentFactory" /> has two roles: Create new
    /// instances of an <see cref="Agent" />, and use <see cref="Map" /> of properties to configure state of new
    /// <see cref="Agent" />s.
    /// </summary>
    public abstract class AgentFactory
    {
        //// ----------------------------------------------------------------------------------------------------------

        private const string ConfigPath = "config";

        private readonly string agentConfigurationFileName;

        private readonly bool configRequired;

        //// ----------------------------------------------------------------------------------------------------------
		
        protected AgentFactory(string agentConfigFileName)
        {
            agentConfigurationFileName = agentConfigFileName;
            configRequired = true;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        protected AgentFactory()
        {
            agentConfigurationFileName = null;
            configRequired = false;
        }

        //// ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Return a new instance of the appropriate <see cref="Agent" /> subclass, configured with information
        /// extracted from the <see cref="properties" />, a <see cref="Map" /> of configuration keys and values.
        /// The keys and values are the result of processing the file referred to by
        /// <see cref="GetAgentConfigFileName" />.
        /// The specific keys and legal values are specific to the domain of the agent.
        /// Since the values come in as <see cref="object" />s, casting and conversion may be required.
        /// </summary>
        /// <param name="properties">The map of property values for creating a configured <see cref="Agent" /></param>
        /// <returns>An instance of <see cref="Agent" /> configured with the specified property values</returns>
        public abstract Agent CreateConfiguredAgent(Dictionary<string, object> properties);

        //// ----------------------------------------------------------------------------------------------------------
		
        public string GetAgentConfigFileName()
        {
            return agentConfigurationFileName;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void CreateConfiguredAgents(Runner runner)
        {
            if (configRequired)
            {
                JArray json = ReadJsonFile(GetAgentConfigFileName());

                for (var i = 0; i < json.Count; i++)
                {
                    ////JObject obj = (JObject)json[i];

                    ////var map = (Dictionary<string, object>)obj;

                    CreateAndRegister(runner, new Dictionary<string, object>());
                }
            }
            else
            {
                CreateAndRegister(runner, new Dictionary<string, object>());
            }
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private void CreateAndRegister(Runner runner, Dictionary<string, object> map)
        {
            var agent = CreateConfiguredAgent(map);

            Context.GetLogger().Debug("Created agent: " + agent);

            runner.Register(agent);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private JArray ReadJsonFile(string filename)
        {
            object parseResult = null;

            var configFileContent = GetConfigurationFile(filename);

            var reader = JsonConvert.DeserializeObject(configFileContent);

            // TODO: finish this method!
            return new JArray();
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static string GetConfigurationFile(string filename)
        {
            var path = Path.Combine(ConfigPath, filename);

            if (!File.Exists(path))
                LogAndThrow("Cannot find config file " + filename);

            return File.ReadAllText(path);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static void LogAndThrow(string error)
        {
            Context.GetLogger().Fatal(error);

            throw new ConfigurationErrorsException(error);
        }
 
        //// ---------------------------------------------------------------------------------------------------------- 
    }
}
