using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ThirdPartyLibTest.NewtonJsonTests
{
    [TestClass]
    public class SearializeTest
    {
        [TestMethod]
        public void TestSerializeEnum()
        {
            var obj = new TestPoco
            {
                Nature = NatureType.Good
            };

            var settings = new JsonSerializerSettings {
                ContractResolver = new DefaultContractResolver(),

            };
            var json = JsonConvert.SerializeObject(obj);

            Assert.IsFalse(json.Contains("Good"));
        }
    }
}
