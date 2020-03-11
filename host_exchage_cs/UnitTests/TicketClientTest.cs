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
    class TicketClientTest
    {
        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory("C:\\Projects\\host_exchage_cs\\host_exchage_cs\\bin\\Release");
            App.TestInit();
        }

        [Test]
        public void Check_btc_addresstype_TicketClient()
        {
            TicketClientSettingsData client = App
                .settings
                .data
                .ticket_clients
                .Where(
                    el => el.btc_addresstype == TicketClientSettingsData.IS_GET_FROM_TICKET_CLIENT
                )
                .First();

            Assert.IsTrue(client.btc_addresstype == TicketClientSettingsData.IS_GET_FROM_TICKET_CLIENT);

            App.LAST_PARSED_RATE = new RateParsedData()
            {
                client = client
            };

            var response = App.ticketClient.GetTypeBtcAddress();

            Assert.IsTrue(response.target_currency_id > 0, "error: " + response.toJson());
        }
    }
}
