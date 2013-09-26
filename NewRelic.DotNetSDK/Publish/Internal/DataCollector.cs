using NewRelic.DotNetSDK.Publish.Binding;

namespace NewRelic.DotNetSDK.Publish.Internal
{
    public class DataCollector
    {
        //// ----------------------------------------------------------------------------------------------------------

        public const string MetricPrefix = "Component/";
        public const string DefaultHost = "host";
        public const int DefaultPid = 0;

        private readonly Agent agent;
        private readonly Context context;
        private readonly ComponentData componentData;

        private Request request;
        
        //// ---------------------------------------------------------------------------------------------------------- 
        
        public DataCollector(Agent agent)
        {
            this.agent = agent;

            context = new Context { AgentData = { Host = DefaultHost, Version = agent.GetVersion(), Pid = DefaultPid } };

            componentData = context.CreateComponent();
            componentData.Guid = agent.GetGuid();
            componentData.Name = agent.GetComponentHumanLabel();
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public Agent GetAgent()
        {
            return agent;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public Context GetContext()
        {
            return context;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void SetRequest(Request request)
        {
            this.request = request;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void AddData(string metricName, string units, float value)
        {
            request.AddMetric(componentData, GetMetricFullName(metricName, units), value);
        }

        //// ----------------------------------------------------------------------------------------------------------

        public void AddData(string metricName, string units, int count, float value, float minValue, float maxValue, float sumOfSquares)
        {
            request.AddMetric(componentData, GetMetricFullName(metricName, units), count, value, minValue, maxValue, sumOfSquares);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static string GetMetricFullName(string metricName, string units)
        {
            return string.Format("{0}{1}[{2}]", MetricPrefix, metricName, units);
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}