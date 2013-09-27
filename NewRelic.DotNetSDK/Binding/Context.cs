using System;
using System.Collections.Generic;
using System.Net;

using log4net;

namespace NewRelic.DotNetSDK.Binding
{
    public class Context
    {
        //// ----------------------------------------------------------------------------------------------------------

        private const string ServiceUri = "https://platform-api.newrelic.com/platform/v1/metrics";
        private static readonly string LogConfigFile = "log4net.config";
        private static readonly string LoggerName = "com.newrelic.metrics.publish";
	
	    private static readonly long AggregationLimit = TimeSpan.FromMinutes(20).Milliseconds;
        private static readonly int ConnectionTimeout = TimeSpan.FromSeconds(20).Milliseconds;
	
	    public string LicenseKey { get; set; }

	    public AgentData AgentData { get; set; }
	
	    private string serviceUri = ServiceUri;
	    private bool sslHostVerification = true;
        
        private static ILog logger;

	    private readonly LinkedList<ComponentData> components;
	
	    private Request lastRequest;
	    private DateTime aggregationStartedAt;

        //// ----------------------------------------------------------------------------------------------------------

        public static ILog GetLogger()
        {
            if (logger == null)
                InitializeLogger();

            return logger;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public static void SetLogger(ILog log)
        {
            logger = log;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static void InitializeLogger()
        {
            logger = LogManager.GetLogger(typeof(Context));
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public Context()
        {
            AgentData = new AgentData();
            components = new LinkedList<ComponentData>();
            lastRequest = new Request(this);
            aggregationStartedAt = DateTime.Now;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public Request CreateRequest()
        {
            if (IsPastAggregationLimit())
            {
                lastRequest = new Request(this);

                foreach (var component in components)
                {
                    component.SetLastSuccessfulReportedAt(null);
                }
            }
            else if (IsLastRequestDelivered())
            {
                lastRequest = new Request(this);
            }

            return lastRequest;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private bool IsLastRequestDelivered()
        {
            return lastRequest.IsDelivered();
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private bool IsPastAggregationLimit()
        {
            var aggregationDuration = DateTime.Now - aggregationStartedAt;

            return aggregationDuration.Milliseconds > AggregationLimit;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public void SetAggregationStartedAt(DateTime startedAt)
        {
            aggregationStartedAt = startedAt;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public DateTime GetAggregationStartedAt()
        {
            return aggregationStartedAt;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public ComponentData CreateComponent()
        {
            var componentData = new ComponentData();

            Add(componentData);

            return componentData;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public LinkedList<ComponentData>.Enumerator GetComponents()
        {
            return components.GetEnumerator();
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public string GetServiceUri()
        {
            return serviceUri;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        internal void SetServiceUri(string uri)
        {
            serviceUri = uri;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        internal void SetSslHostVerification(bool hostVerification)
        {
            sslHostVerification = hostVerification;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private void Add(ComponentData data)
        {
            components.AddLast(data);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public HttpWebRequest CreateUrlConnectionForOutput()
        {
            var serviceUrl = new Uri(serviceUri);

            logger.DebugFormat("Metric service url: {0}", serviceUrl);

            var connection = (HttpWebRequest)WebRequest.Create(serviceUrl);

            connection.Method = "POST";
            connection.Headers.Add("X-License-Key", LicenseKey);
            connection.Headers.Add("Content-Type", "application/json");
            connection.Headers.Add("Accept", "application/json");
            connection.Timeout = ConnectionTimeout;
            
            //// Copied from Java implementation but not sure if we need this or how to port to .NET
            ////
            ////if (connection instanceof HttpsURLConnection && !sslHostVerification) {
            ////    // ssl hostname verifier verifies any host
            ////    ((HttpsURLConnection) connection).setHostnameVerifier(new HostnameVerifier() {
            ////        @Override
            ////        public boolean verify(String hostname, SSLSession session) {
            ////            return true;
            ////        }
            ////    });
            ////}

            return connection;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public Dictionary<string, object> Serialize(Request request)
        {
            var output = new Dictionary<string, object> { { "agent", AgentData.Serialize() } };

            var componentsOutput = new LinkedList<Dictionary<string, object>>();

            foreach (var component in components)
            {
                componentsOutput.AddLast(component.Serialize(request));
            }

            output.Add("components", componentsOutput);

            return output;
        }

        //// ----------------------------------------------------------------------------------------------------------
    }
}