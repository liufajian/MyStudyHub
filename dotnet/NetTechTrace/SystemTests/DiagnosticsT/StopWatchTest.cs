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

                Assert.IsTrue(sw.ElapsedMilliseconds > i * 1000 && sw.ElapsedMilliseconds < i * 1015);
            }

            Assert.IsTrue(sw.ElapsedMilliseconds > 10 * 1000 && sw.ElapsedMilliseconds < 10 * 1015);

            Console.WriteLine("验证成功");
        }

        [TestMethod]
        public void TestTimeout()
        {
            var timeoutSeconds = 30;
            var timeoutSpan = TimeSpan.FromSeconds(timeoutSeconds);
            var sw = Stopwatch.StartNew();

            for (var i = 1; i <= 100; i++)
            {
                Thread.Sleep(1001);

                if (sw.Elapsed > timeoutSpan)
                {
                    sw.Stop();
                    Assert.AreEqual(i, 30);
                    break;
                }
            }

            Assert.IsTrue(sw.ElapsedMilliseconds > 30 * 1000 && sw.ElapsedMilliseconds < 30 * 1015);

            Console.WriteLine("验证成功");
        }
    }
}
