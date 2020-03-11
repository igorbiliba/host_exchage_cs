using host_exchage_cs.Components;
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
    class BTCAddrStorageTest
    {
        [SetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory("C:\\Projects\\host_exchage_cs\\host_exchage_cs\\bin\\Release");
            App.TestInit();
        }

        [Test]
        public void TestCnt()
        {
            App.btcAddrStorage.UpdateAddressList();
            Assert.IsTrue(App.btcAddrStorage.list.Count() == App.settings.data.storage_btc_addr_types.Split(',').Count());

            foreach (var item in App.btcAddrStorage.list)
            {
                Assert.IsTrue(item.list.Count == App.settings.data.storage_btc_addr_cnt);
            }

            App.btcAddrStorage.UpdateAddressList();

            foreach (string type in App.settings.data.storage_btc_addr_types.Replace(" ", "").Split(','))
            {
                Assert.IsTrue(
                    GetCnt(type) == App.settings.data.storage_btc_addr_cnt
                );

                App.btcAddrStorage.GetOne(type);

                Assert.IsFalse(
                    GetCnt(type) == App.settings.data.storage_btc_addr_cnt
                );

                App.btcAddrStorage.UpdateAddressList();
                Assert.IsTrue(
                    GetCnt(type) == App.settings.data.storage_btc_addr_cnt
                );
            }
        }

        int GetCnt(string type)
        {
            return App
                .btcAddrStorage
                .list
                .Find(el => el.name == BTCAddrStorage.ModType(type))
                .list.Count;
        }
    }
}
