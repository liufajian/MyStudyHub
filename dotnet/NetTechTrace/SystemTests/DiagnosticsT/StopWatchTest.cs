using System.Diagnostics;

namespace NetTechTrace.SystemTests.DiagnosticsT
{
    [TestClass]
    public class StopWatchTest
    {
        [TestMethod]
        public void Test1()
        {
            var sw = Stopwatch.StartNew();

            for (var i = 1; i <= 10; i++)
            {
                Thread.Sleep(1000);

                Assert.IsTrue(sw.ElapsedMilliseconds > i * 1000 && sw.ElapsedMilliseconds < i * 1000 + 100);
            }

            Assert.IsTrue(sw.ElapsedMilliseconds > 10 * 1000 && sw.ElapsedMilliseconds < 10 * 1000 + 100);

            Console.WriteLine("验证成功");
        }
    }
}
