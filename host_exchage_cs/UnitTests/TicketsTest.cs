using host_exchage_cs.Components;
using host_exchage_cs.Components.TicketClient.Responses;
using host_exchage_cs.Data;
using host_exchage_cs.Host.Responses;
using host_exchage_cs.Models;
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
    class TicketsTest
    {
        double rubAmount;
        string phone;
        string btcAddr;

        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory("C:\\Projects\\host_exchage_cs\\host_exchage_cs\\bin\\Release");
            DB.RemoveDB();
            App.TestInit();

            rubAmount = 5000;
            phone     = App.test.data.ticket.account;
            btcAddr   = App.test.data.btc_address;
        }

        [Test]
        public void CreateTicketByEveryTicketsClients()
        {
            //List<string> fakeBtcAddress = new List<string>() {
            //    "3K49uLwB1BzeWvcXyeUA94eACt2VUNutUk",
            //    "3FamfsgeKXYoSayoEuf6v5UweJjKNmgA1K",
            //    "31pSYyCDgTGsnPTb4vhnyi4uoLyRsfxwMy",
            //    "37CARoxAzyxw4Aoihh9GGc7XPimLT5h3Bn",
            //    "3LjjL73ooS3JZ2VeAPVdAa7qSbnQD3ATQD",
            //    "34L4kJA7WHCvXhd6NANzwht9cDyRmUfbE8",
            //    "37885rHzhpJRpoTpQGGDor5TptWixGGgPM",
            //    "3KrSmGYPME6P4RmxfXT2cyE5rZAuT1MJAK",
            //    "3639wssUVEQNzZRZMfi3x6a8YnP8nUDLcL",
            //    "3MfnGpHmQHA2ddSuAZaAw5dc5cZPmSPaUs",
            //    "37AiPqapUGkV4vptQePNhQsv48CeSjD8Bg",
            //    "3HUe1raRC89ZyUGzW9Qn4nkVLsWzvfJDFK",
            //    "3LMSk8aV2Ytu5xfH5j9jVh662CTrQH4ySA",
            //    "3H2YeAebJhFiPf1PED7GQNjkiGqtHnECeb",
            //    "32WaSsvyKRWeBN4wSZxst14KrD2Ynnqb1y",
            //    "332LztnSV5BHyBLmZ4a8fYjTp89GNoqvdJ",
            //    "3FqRm8W6ifRwB2Y93W3LAjqWF5KtMcNKkq",
            //    "3Asxj5RN3kYhhUr5icxSsfx6BxDd79tcon"
            //};

            ////получаем все тикеты из настроек
            //List<RateParsedData> list = new List<RateParsedData>();
            //foreach (var client in App.settings.data.ticket_clients)
            //{
            //    var bullet = new RateParsedData()
            //    {
            //        client = client
            //    };
            //    bullet.ParseOnClient();
            //    list.Add(bullet);
            //}

            //list.Reverse();
            ////устанавливаем по очереди тикеты и создаем заявки
            //foreach (RateParsedData item in list)
            //{
            //    string phoneFrom = "+790";
            //    for (int i = 0; i < 8; i++)
            //        phone += new Random()
            //                    .Next(0, 9)
            //                    .ToString();

            //    btcAddr = fakeBtcAddress.Last();
            //    fakeBtcAddress.Remove(btcAddr);

            //    App.LAST_PARSED_RATE = item;

            //    TicketClientCreateResponse ticket;
            //    try
            //    {
            //        //ticket = App.ticketClient.Create(rubAmount, phoneFrom, btcAddr);
            //    } catch(Exception ex)
            //    {
            //        Assert.Fail("Не создается заявка через " + App.LAST_PARSED_RATE.GetClientNameOnPath() + ", " + ex.Message);
            //        ticket = new TicketClientCreateResponse();
            //    }
            //    string hash = (new BtcAddressModel(App.db))
            //        .Create(
            //            0,
            //            btcAddr,
            //            ticket.account, //phone to
            //            ticket.comment,
            //            ticket.btc_amount,
            //            App.LAST_PARSED_RATE.GetClientNameOnPath(),
            //            phoneFrom,
            //            "test",
            //            ticket.email
            //        );

            //    Assert.IsNotEmpty(hash, "Не создается hash через " + App.LAST_PARSED_RATE.GetClientNameOnPath());

            //    CreateResponse response = new CreateResponse()
            //    {
            //        account = ticket.account,
            //        comment = ticket.comment,
            //        hash = hash,
            //        btc_amount = ticket.btc_amount.ToString()
            //    };

            //    Assert.IsTrue(response.IsValide(), "Не создается заявка через " + App.LAST_PARSED_RATE.GetClientNameOnPath() + ": " + response.toJson());
            //}
        }
    }
}
