using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NewRelic.DotNetSDK.Runners;

namespace NewRelic.DotNetSDK.Tests.Runners
{
    [TestClass]
    public class RunnerTests
    {
        //// ----------------------------------------------------------------------------------------------------------
		 
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            //var mock = new Mock<IRunnable>();

            // Act
            var runner = new Runner(new PollAgentsRunnable());

            runner.Register(new OneCycleAgent("one.cycle.agent", "1.2.3"));

            runner.SetupAndRun();

            // Assert
            //mock.Verify(r => r.Run(null), Times.AtLeastOnce());
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}
