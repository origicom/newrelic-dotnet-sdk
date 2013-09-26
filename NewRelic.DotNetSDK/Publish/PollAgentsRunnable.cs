using System;
using System.Collections.Generic;
using System.Diagnostics;

using NewRelic.DotNetSDK.Publish.Binding;

namespace NewRelic.DotNetSDK.Publish
{
    internal class PollAgentsRunnable
    {
        //// ----------------------------------------------------------------------------------------------------------

        private readonly LinkedList<Agent> agents; 

        //// ----------------------------------------------------------------------------------------------------------
		 
        public PollAgentsRunnable(LinkedList<Agent> agents)
        {
            this.agents = agents;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void Run()
        {
            if (agents == null)
                return;

            Context.GetLogger().Debug("Harvest and report data");

            try
            {
                foreach (var agent in agents)
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
    }
}