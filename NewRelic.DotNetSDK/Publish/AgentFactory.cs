using System.Collections.Generic;
using System.Configuration;
using System.IO;

using NewRelic.DotNetSDK.Publish.Binding;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewRelic.DotNetSDK.Publish
{
    public abstract class AgentFactory
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        private readonly string agentConfigurationFileName;

        private bool configRequired = true;

        private static readonly string ConfigPath = "config";

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
		
        private string GetConfigurationFile(string filename)
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
