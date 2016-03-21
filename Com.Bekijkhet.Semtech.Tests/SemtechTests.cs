using NUnit.Framework;
using System;

namespace Com.Bekijkhet.Semtech.Tests
{
    [TestFixture ()]
    public class SemtechTests
    {
        [Test ()]
        public void Identifier_PUSH_DATA_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PUSH_DATA, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x00,0x00}));
        }
        [Test ()]
        public void Identifier_PUSH_ACK_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PUSH_ACK, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x01,0x00}));
        }
        [Test ()]
        public void Identifier_PULL_DATA_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PULL_DATA, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x02,0x00}));
        }
        [Test ()]
        public void Identifier_PULL_RESP_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PULL_RESP, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x03,0x00}));
        }
        [Test ()]
        public void Identifier_PULL_ACK_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PULL_ACK, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x04,0x00}));
        }

        [Test ()]
        [ExpectedException( typeof( InvalidIdentifierException ))]
        public void Identifier_InvalidIdentifierException_Test ()
        {
            var x = new SemtechImpl ();

            Assert.AreEqual(Identifier.PUSH_DATA, x.GetIdentifier(new byte[5]{0x00,0x00,0x00,0x05,0x00}));
        }
    }
}

