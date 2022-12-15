using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTechTrace.SystemTests.DataTypesTests
{
    [TestClass]
    public class EnumTypeTest
    {
        [TestMethod]
        public void TestEnumDefault()
        {
            EnumType1 t1 = default;
            EnumType2 t2 = default;

            Assert.IsTrue(t1 == EnumType1.None);
            Assert.IsTrue(t2 == 0);
            Assert.IsTrue(t2 != EnumType2.None);
        }

        enum EnumType1
        {
            None = 0,

            One = 1,

            Two = 2
        }

        enum EnumType2
        {
            None = 1,

            One = 11,

            Two = 22
        }
    }
}
