using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewRelic.DotNetSDK.Publish.Binding
{
    public class Request
    {
        //// ----------------------------------------------------------------------------------------------------------

        private const string OkStatus = "ok";
        private const string DisableNewRelic = "DISABLE_NEW_RELIC";
        private const int ExitCode = 1;
        private const string Status = "status";

        private readonly Context context;
        private readonly Dictionary<ComponentData, LinkedList<MetricData>> metrics = new Dictionary<ComponentData, LinkedList<MetricData>>();

        private bool delivered = false;

        //// ----------------------------------------------------------------------------------------------------------

        public Request(Context context)
        {
            this.context = context;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public MetricData AddMetric(ComponentData component, string name, float value)
        {
            var metricData = AddMetric(component, new MetricData(name, value));
            
            return metricData;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public MetricData AddMetric(ComponentData component, string name, int count, float value, float minValue, float maxValue, float sumOfSquares)
        {
            var metricData = AddMetric(component, new MetricData(name, count, value, minValue, maxValue, sumOfSquares));

            return metricData;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public void Deliver()
        {
            // do not send an empty request
            if (metrics.Count == 0)
            {
                Context.GetLogger().Debug("No metrics were reported for this poll cycle");
            }
            else
            {
                var logger = Context.GetLogger();

                try
                {
                    var connection = context.CreateUrlConnectionForOutput();

                    using (var outputStream = connection.GetRequestStream())
                    {
                        var data = Serialize();

                        var json = JsonConvert.SerializeObject(data);

                        logger.DebugFormat("Sending JSON: {0}", json);

                        var bytes = Encoding.UTF8.GetBytes(json);

                        outputStream.Write(bytes, 0, bytes.Length);
                    }

                    using (var response = connection.GetResponse())
                    {
                        // process and log response from the collector
                        ProcessResponse((HttpWebResponse)response);
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal("An error occurred communicating with the New Relic service", ex);
                }
            }
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private void ProcessResponse(HttpWebResponse connection)
        {  	
    	    var responseCode = connection.StatusCode;
  
            // Do not log 503 responses
            if (IsCollectorUnavailable(responseCode)) 
            {
        	    Context.GetLogger().Debug("Collector temporarily unavailable... continuing");
            }
            else 
            {
        	    // read server response
        	    var responseBody = GetServerResponse(connection);
        	
        	    if (IsResponseEmpty(responseBody)) 
                {
        		    Context.GetLogger().Info("Failed server response: no response");
        	    }
        	    else if (IsRemotelyDisabled(responseCode, responseBody)) 
                {
        		    // Remote disabling by New Relic -- exit
        	        Context.GetLogger().Fatal("Agent has been disabled remotely by New Relic");

        		    throw new ApplicationException("Agent has been disabled remotely by New Relic");
                }
        	    else if (IsResponseOk(responseCode, responseBody)) 
                {
        		    Context.GetLogger().DebugFormat("Server response: {0}, {1}", responseCode, responseBody);

        		    delivered = true;

                    var deliveredAt = DateTime.Now;

        		    context.SetAggregationStartedAt(deliveredAt);
        		    
                    // update last successful timestamps
        		    UpdateComponentTimestamps(deliveredAt);
                }
                else 
                {
        		    // All other response codes will fail
        		    Context.GetLogger().FatalFormat("Failed server response: {0}, {1}", responseCode, responseBody);
        	    }
            }
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static string GetServerResponse(HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);

                    return reader.ReadToEnd();
                }
            }

            return null;
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        private static bool IsCollectorUnavailable(HttpStatusCode responseCode)
        {
            return responseCode == HttpStatusCode.ServiceUnavailable;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static bool IsResponseEmpty(string responseBody)
        {
            return string.IsNullOrEmpty(responseBody);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static bool IsRemotelyDisabled(HttpStatusCode responseCode, string responseBody)
        {
            return responseCode == HttpStatusCode.Forbidden && responseBody == DisableNewRelic;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static bool IsResponseOk(HttpStatusCode responseCode, string responseBody)
        {
            var statusMessage = GetStatusMessage(responseBody);

            return responseCode == HttpStatusCode.OK && statusMessage == OkStatus;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static string GetStatusMessage(string responseBody)
        {
            var jsonObject = JObject.Parse(responseBody);

            if (jsonObject != null)
                return (string)jsonObject[Status];

            return null;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public bool IsDelivered()
        {
            return delivered;
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        private void UpdateComponentTimestamps(DateTime deliveredAt)
        {
            foreach (var component in metrics.Keys)
            {
                component.SetLastSuccessfulReportedAt(deliveredAt);
            }
        }

        //// ----------------------------------------------------------------------------------------------------------

        private Dictionary<string, object> Serialize()
        {
            return context.Serialize(this);
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        public LinkedList<MetricData> GetMetrics(ComponentData component)
        {
            if (!metrics.ContainsKey(component))
            {
                metrics.Add(component, new LinkedList<MetricData>());
            }

            return metrics[component];
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private MetricData AddMetric(ComponentData component, MetricData metric)
        {
            Context.GetLogger().DebugFormat("{0} : {1}", component, metric);

            var localMetrics = GetMetrics(component);

            if (localMetrics.Contains(metric))
            {
                Aggregate(metric, localMetrics);
            }
            else
            {
                localMetrics.AddLast(metric);
            }

            return metric;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private static void Aggregate(MetricData metric, IEnumerable<MetricData> existingMetrics)
        {
            var existingMetric = existingMetrics.SingleOrDefault(m => m.Name == metric.Name);

            if (existingMetric != null)
                existingMetric.AggregateWith(metric);
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}
