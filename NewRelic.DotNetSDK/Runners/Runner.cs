using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

using NewRelic.DotNetSDK.Binding;
using NewRelic.DotNetSDK.Configuration;

using Ninject;

namespace NewRelic.DotNetSDK.Runners
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

        private readonly HashSet<AgentFactory> factories;

        private readonly IRunnable runner;

        private int pollIntervalSeconds = 60;

        private Timer timer;

        //// ----------------------------------------------------------------------------------------------------------

        public Runner(IRunnable runnable)
        {
            agents = new LinkedList<Agent>();

            factories = new HashSet<AgentFactory>();

            config = new SDKConfiguration();

            runner = runnable;

            //try
            //{
            //    //config = new SDKConfiguration();
            //}
            //catch (Exception ex)
            //{
            //    Context.GetLogger().Fatal(ex.Message, ex);
            //    throw new ApplicationException("Error initializing configuration", ex);
            //}
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
        /// Setup the <see cref="Runner" /> and run in a loop that will never return.
        /// Add an <see cref="AgentFactory" /> or register <see cref="Agent" />s directly before calling.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Throws if the Runner was not correctly configured</exception>
        public void SetupAndRun()
        {
            SetupAgents();

            pollIntervalSeconds = config.GetPollInterval();

            runner.Agents = agents;

            var pollIntervalMilliseconds = pollIntervalSeconds * 1000;

            timer = new Timer(runner.Run, null, 0, pollIntervalMilliseconds);

            while (true)
            {
            }
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