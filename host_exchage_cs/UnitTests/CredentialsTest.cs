using host_exchage_cs.Components;
using host_exchage_cs.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.UnitTests
{
    [TestFixture]
    class CredentialsTest
    {
        

        [SetUp]
        public void CredentialsTestInit()
        {
            Directory.SetCurrentDirectory("C:\\Projects\\host_exchage_cs\\host_exchage_cs\\bin\\Release");
        }

        [Test]
        public void CheckAllowTest()
        {
            Settings settings = new Settings();
            settings.data.allow_methods = "method1, method3";
            Credentials credentials = new Credentials(settings);

            Assert.IsFalse(credentials.IsAllow("method2"), "disallow не работает");
            Assert.IsTrue(credentials.IsAllow("method1"), "allow не работает");
        }
    }
}
