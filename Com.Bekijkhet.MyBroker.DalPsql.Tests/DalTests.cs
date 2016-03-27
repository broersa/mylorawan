using NUnit.Framework;
using System;

namespace Com.Bekijkhet.MyBroker.DalPsql.Tests
{
    [TestFixture()]
    public class DalTests
    {
        [Test()]
        public void TestCase()
        {
            var dal = new Dal(new DalConfig(Environment.GetEnvironmentVariable("MYBROKER_DB")));
            var ses = dal.GetSessionOnDeviceDevNonceActive(1, "dc6a").Result;
            Assert.AreEqual(ses, null);
        }
    }
}

