using System;
using System.Collections.Generic;
using System.Configuration;

using NewRelic.DotNetSDK.Publish.Binding;
using NewRelic.DotNetSDK.Publish.Configuration;

namespace NewRelic.DotNetSDK.Publish
{
    /// <summary>
    /// The main entry point for executing the SDK.
    /// Add an <see cref="AgentFactory" /> to create an <see cref="Agent" /> 
    /// or register an <see cref="Agent" /> directly. The <see cref="Runner" /> will poll <see cref="Agent" />s 
    /// in a loop that never returns.
    /// </summary>
    public class Runner
    {
        //// ----------------------------------------------------------------------------------------------------------

        private readonly SDKConfiguration config;

        private readonly LinkedList<Agent> agents;

        private readonly HashSet<AgentFactory> factories = new HashSet<AgentFactory>();

        private int pollInterval = 60;

        //// ----------------------------------------------------------------------------------------------------------

        public Runner()
        {
            agents = new LinkedList<Agent>();

            try
            {
                config = new SDKConfiguration();
            }
            catch (Exception ex)
            {
                Context.GetLogger().Fatal(ex.Message, ex);
                throw new ApplicationException("Error initializing configuration", ex);
            }
        }

        //// ----------------------------------------------------------------------------------------------------------

        public void Add(AgentFactory factory)
        {
            factories.Add(factory);
        }

        //// ----------------------------------------------------------------------------------------------------------

        public void Register(Agent agent)
        {
            agents.AddLast(agent);
        }

        //// ----------------------------------------------------------------------------------------------------------

        public SDKConfiguration GetConfiguration()
        {
            return config;
        }

        //// ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Setup the {@code Runner} and run in a loop that will never return.
        /// Add an {@link AgentFactory} or register {@link Agent}s before calling.
        /// @throws ConfigurationException if the {@link Runner} was not configured correctly
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Throws if the Runner was not correctly configured</exception>
        public void SetupAndRun()
        {
            SetupAgents();

            pollInterval = config.GetPollInterval();

            var pollAgentsRunnable = new PollAgentsRunnable(agents);

            // TODO: Urgent! This needs moving onto a thread and into a loop, at the moment it will run once and return
            // TODO: Need to re-run this every pollInterval
            pollAgentsRunnable.Run();
        }

        //// ----------------------------------------------------------------------------------------------------------

        private void CreateAgents()
        {
            foreach (var factory in factories)
                factory.CreateConfiguredAgents(this);
        }

        //// ----------------------------------------------------------------------------------------------------------

        private void SetupAgents()
        {
            Context.GetLogger().Debug("Setting up agents to be run");

            CreateAgents();

            if (config.GetServiceUri() != null)
            {
                Context.GetLogger().Info("Using host: " + config.GetServiceUri());
            }

            foreach (var agent in agents)
            {
                agent.PrepareToRun();
                agent.SetupMetrics();

                agent.GetCollector().GetContext().LicenseKey = config.GetLicenseKey();

                if (config.GetServiceUri() != null)
                {
                    agent.GetCollector().GetContext().SetServiceUri(config.GetServiceUri());
                }

                agent.GetCollector().GetContext().SetSslHostVerification(config.IsSslHostVerificationEnabled());
            }
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}