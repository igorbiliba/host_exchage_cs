using host_exchage_cs.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.UnitTests
{
    [TestFixture]
    class SettingsTest
    {
        [SetUp]
        public void SettingsTestInit()
        {

        }

        [Test]
        public void ChangeHttsOrHttpByUseSSLFlag()
        {
            //const string address = "localhost:8086";
            //var data = new SettingsData()
            //{
            //    address = address
            //};

            //data.use_ssl_and_rpc_auth = true;
            //Assert.IsTrue(data.address == "https://" + address + "/", "Не применяется https по флагу ssl=true");

            //data.use_ssl_and_rpc_auth = false;
            //Assert.IsTrue(data.address == "http://" + address + "/", "Не применяется http по флагу ssl=false");
        }
    }
}
