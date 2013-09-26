using NewRelic.DotNetSDK.Publish.Binding;
using NewRelic.DotNetSDK.Publish.Internal;

namespace NewRelic.DotNetSDK.Publish
{
    public abstract class Agent
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        private readonly string guid;

        private readonly string version;

        private DataCollector collector;

        //// ----------------------------------------------------------------------------------------------------------
		
        protected Agent(string guid, string version)
        {
            this.guid = guid;
            this.version = version;
        }

        //// ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The Agent will gather and report metrics from this method during every poll cycle. 
        /// It is called by the Runner at a set interval and is run in a loop that never returns.
        /// This method must be overridden by subclasses of Agent.
        /// </summary>
        public abstract void PollCycle();
 
        //// ----------------------------------------------------------------------------------------------------------
		 
        public string GetVersion()
        {
            return version;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public string GetGuid()
        {
            return guid;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public DataCollector GetCollector()
        {
            return collector;
        }

        //// ---------------------------------------------------------------------------------------------------------- 

        /**
         * A human readable label for the component that this {@code Agent} is reporting metrics on. 
         * <p> This method must be overridden by subclasses of {@code Agent}.
         * @return String the component human label
         */

        /// <summary>
        /// A human readable label for the component that this Agent is reporting metrics on. 
        /// This method must be overridden by subclasses of Agent.
        /// </summary>
        /// <returns>The component human label</returns>
        public abstract string GetComponentHumanLabel();

        //// ----------------------------------------------------------------------------------------------------------
		
        /// <summary>
        /// A hook called when the Agent is set up. Subclasses may override but must call base implementation.
        /// </summary>
        public virtual void SetupMetrics()
        {
            Context.GetLogger().Debug("SetupMetrics");
        }

        //// ----------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// A hook called when the Agent is set up. Subclasses may override but must call base implementation.
        /// </summary>
        public virtual void PrepareToRun()
        {
            // This needs to be done after being configured to ensure the binding model created by the DataCollector
            // has the most recent values from this
            collector = new DataCollector(this);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void ReportMetric(string metricName, string units, float value)
        {
            Context.GetLogger().DebugFormat("Reporting metric: {0}", metricName);
            collector.AddData(metricName, units, value);
        }

        //// ----------------------------------------------------------------------------------------------------------

        public void ReportMetric(string metricName, string units, int count, float value, float minValue, float maxValue, float sumOfSquares)
        {
            Context.GetLogger().DebugFormat("Reporting metric: {0}", metricName);
            collector.AddData(metricName, units, count, value, minValue, maxValue, sumOfSquares);
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}