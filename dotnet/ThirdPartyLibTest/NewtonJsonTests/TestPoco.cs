namespace ThirdPartyLibTest.NewtonJsonTests
{
    internal class TestPoco
    {
        public NatureType Nature { get; set; }
    }

    enum NatureType
    {
        None,
        Good,
        Honest,
        Tricky
    }
}
