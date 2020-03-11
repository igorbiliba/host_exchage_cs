using host_exchage_cs.Components;
using host_exchage_cs.Data;
using host_exchage_cs.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.UnitTests
{
    [TestFixture]
    class BtcAddressForwardModelTest
    {
        BtcAddressForwardModel model;
        string from;
        string to;
        int dempingpercent;
        int forwardint = 1;
        string forwardfraction = "2";

        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory("C:\\Projects\\host_exchage_cs\\host_exchage_cs\\bin\\Release");
            App.TestInit();

            model = new BtcAddressForwardModel(App.db);

            var now = Now();
            from = now + "-from";
            to = now + "-to";
            dempingpercent = new Random().Next(1, 5);
        }

        long Now() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

        [Test, Order(1)]
        public void AddUpdate()
        {
            Assert.IsTrue(
                new BtcAddressForwardEntity()
                {
                    forwardfraction = "01"
                }.GetForwardAmount() == 0.01
                , "Ошибка forwardfraction"
            );

            Assert.IsTrue(
                new BtcAddressForwardEntity()
                {
                    forwardint = 2
                }.GetForwardAmount() == 2
                , "Ошибка forwardfraction"
            );

            Assert.IsTrue(
                new BtcAddressForwardEntity()
                {
                    forwardint = 3,
                    forwardfraction = "4"
                }.GetForwardAmount() == 3.4
                , "Ошибка forwardint и forwardfraction"
            );

            model.Create(from, to, dempingpercent, forwardint, forwardfraction);
            var i = model.GetByBtcFrom(from);

            Assert.IsTrue(
                i.btc_from == from &&
                int.Parse(i.id) > 0 &&
                i.btc_to == to &&
                i.dempingpercent == dempingpercent &&
                i.forwardint == forwardint &&
                i.forwardfraction == forwardfraction &&
                i.status == BtcAddressForwardEntity.STATUS_WAIT,
                "Не смог создать"
            );

            Assert.IsTrue(model.UpdateStatus(int.Parse(i.id), BtcAddressForwardEntity.STATUS_SENT), "Не получается обновить статус, внутренний ексепшн");
            var changed = model.GetByBtcFrom(from);

            Assert.IsTrue(i.btc_from == from, "Не смог создать");
            Assert.IsTrue(changed.status == BtcAddressForwardEntity.STATUS_SENT, "Не смог обновить (" + changed.status + ") created_at(" + changed.created_at + ")");
        }
    }
}
