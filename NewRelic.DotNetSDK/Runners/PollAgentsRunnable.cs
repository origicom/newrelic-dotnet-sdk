using System;
using System.Collections.Generic;
using System.Diagnostics;

using NewRelic.DotNetSDK.Binding;

namespace NewRelic.DotNetSDK.Runners
{
    public class PollAgentsRunnable : IRunnable
    {
        //// ----------------------------------------------------------------------------------------------------------
		
        public void Run(object arg)
        {
            if (Agents == null || Agents.Count == 0)
                return;

            Context.GetLogger().Debug("Harvest and report data");

            try
            {
                foreach (var agent in Agents)
                {
                    var request = agent.GetCollector().GetContext().CreateRequest();
                    agent.GetCollector().SetRequest(request);
                    agent.PollCycle();
                    request.Deliver();
                    agent.GetCollector().SetRequest(null); // Make sure we're not reusing the request
                }
            }
            catch (Exception ex)
            {
                Context.GetLogger().Fatal("SEVERE: An error has occurred", ex);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        public LinkedList<Agent> Agents { get; set; }

        //// ----------------------------------------------------------------------------------------------------------		 
    }
}