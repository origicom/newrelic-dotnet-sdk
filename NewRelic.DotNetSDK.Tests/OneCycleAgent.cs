namespace NewRelic.DotNetSDK.Tests
{
    /// <summary>
    /// Simple agent for unit testing
    /// </summary>
    internal class OneCycleAgent : Agent
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        public OneCycleAgent(string guid, string version)
            : base(guid, version)
        {
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        public override void PollCycle()
        {
            ReportMetric("Cycles/Count", "cycles", 2, 5, 2, 3, 25);
        }

        //// ----------------------------------------------------------------------------------------------------------
		 
        public override string GetComponentHumanLabel()
        {
            return "One Cycle Agent";
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}