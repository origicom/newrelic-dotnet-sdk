using System;

namespace NewRelic.DotNetSDK.Publish.Processors
{
    public class EpochCounter : IProcessor
    {
        //// ----------------------------------------------------------------------------------------------------------

        private float lastValue;

        private DateTime? lastTime;
 
        //// ----------------------------------------------------------------------------------------------------------
		 
        public float Process(float value)
        {
            var now = DateTime.Now;

            float thisValue = 0;

            if (lastValue > 0 && lastTime.HasValue && now > lastTime.Value)
            {
                var timeDiffInSeconds = (now - lastTime.Value).Seconds;

                if (timeDiffInSeconds > 0)
                {
                    thisValue = (value - lastValue) / timeDiffInSeconds;
                }
            }

            lastValue = value;
            lastTime = now;

            return thisValue;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}