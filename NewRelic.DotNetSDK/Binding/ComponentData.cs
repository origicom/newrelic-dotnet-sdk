using System;
using System.Collections.Generic;

namespace NewRelic.DotNetSDK.Binding
{
    public class ComponentData
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        private const int DefaultDuration = 60;

        //// ----------------------------------------------------------------------------------------------------------
		
        public string Name { get; set; }

        public string Guid { get; set; }

        private DateTime? lastSuccessfulReportedAt;
 
        //// ----------------------------------------------------------------------------------------------------------
		 
        public void SetLastSuccessfulReportedAt(DateTime? reportedAt)
        {
            lastSuccessfulReportedAt = reportedAt;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public Dictionary<string, object> Serialize(Request request)
        {
            var output = new Dictionary<string, object>
                {
                    { "name", Name },
                    { "guid", Guid },
                    { "duration", CalculateDuration() }
                };

            var metricsOutput = new Dictionary<string, object>();

            output.Add("metrics", metricsOutput);

            foreach (var metric in request.GetMetrics(this))
            {
                metric.Serialize(metricsOutput);
            }

            return output;
        }

        //// ----------------------------------------------------------------------------------------------------------
		
        private int CalculateDuration()
        {
            if (lastSuccessfulReportedAt == null)
                return DefaultDuration;

            var now = DateTime.Now;
            var lastReportedAt = lastSuccessfulReportedAt.Value;

            var duration = now - lastReportedAt;

            var durationInSeconds = (int)Math.Ceiling(duration.TotalMilliseconds / 1000);

            return durationInSeconds;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return string.Format("ComponentData({0}:{1})", Name, Guid);
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}