using System.Collections.Generic;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NewRelic.DotNetSDK.Tests
{
    [TestClass]
    public class AgentTests
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        [TestMethod]
        public void AgentTest_OneCycle_ExpectSuccess()
        {
            /************ Arrange ************/
            var agent = new OneCycleAgent("com.test.onecycle", "1.2.3");
            agent.PrepareToRun();

            var request = agent.GetCollector().GetContext().CreateRequest();
            agent.GetCollector().SetRequest(request);

            /************ Act ****************/
            agent.PollCycle();

            var serializeMethod = request.GetType().GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

            var serializedRequest = (Dictionary<string, object>)serializeMethod.Invoke(request, null);

            var expected = new Dictionary<string, object>();
            var expectedAgent = new Dictionary<string, object>
                {
                    { "host", "host" },
                    { "pid", 0 },
                    { "version", "1.2.3" }
                };

            expected.Add("agent", expectedAgent);

            var expectedComponents = new LinkedList<Dictionary<string, object>>();

            var expectedComponent = new Dictionary<string, object>
                {
                    { "guid", "com.test.onecycle" },
                    { "name", "One Cycle Agent" },
                    { "duration", 60 }
                };

            var expectedMetric = new[] { 5f, 2, 2f, 3f, 25f };

            var expectedMetrics = new Dictionary<string, object>
                {
                    { "Component/Cycles/Count[cycles]", expectedMetric }
                };

            expectedComponent.Add("metrics", expectedMetrics);

            expectedComponents.AddLast(expectedComponent);
            expected.Add("components", expectedComponents);

            /************ Assert *************/
            Assert.AreEqual(expected.Count, serializedRequest.Count);

            var actualAgent = (Dictionary<string, object>)serializedRequest["agent"];
            
            Assert.AreEqual(expectedAgent["host"], actualAgent["host"]);
            Assert.AreEqual(expectedAgent["pid"], actualAgent["pid"]);
            Assert.AreEqual(expectedAgent["version"], actualAgent["version"]);
            
            var actualComponents = (LinkedList<Dictionary<string, object>>)serializedRequest["components"];

            Assert.AreEqual(expectedComponents.First.Value["name"], actualComponents.First.Value["name"]);
            Assert.AreEqual(expectedComponents.First.Value["guid"], actualComponents.First.Value["guid"]);
            Assert.AreEqual(expectedComponents.First.Value["duration"], actualComponents.First.Value["duration"]);

            var actualMetrics = (Dictionary<string, object>)actualComponents.First.Value["metrics"];

            var actualMetric = (List<float>)actualMetrics["Component/Cycles/Count[cycles]"];

            Assert.AreEqual(expectedMetric[0], actualMetric[0]);
            Assert.AreEqual(expectedMetric[1], actualMetric[1]);
            Assert.AreEqual(expectedMetric[2], actualMetric[2]);
            Assert.AreEqual(expectedMetric[3], actualMetric[3]);
            Assert.AreEqual(expectedMetric[4], actualMetric[4]);
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}