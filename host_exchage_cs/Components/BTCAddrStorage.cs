using host_exchage_cs.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Components
{
    public class BTCAddrItems
    {
        public string name = "";
        public List<string> list = new List<string>();
    }

    public class BTCAddrStorage
    {
        public const string DEFAULT_TYPE = "p2sh-segwit";

        public List<BTCAddrItems> list = new List<BTCAddrItems>();
        public int CNT;

        public void Init(string[] types)
        {
            foreach (var type in types)
                list.Add(new BTCAddrItems() { name = ModType(type) });
        }

        public void UpdateAddressList()
        {
            bool needUpdate = false;

            for(int i = 0; i < list.Count; i++)
            {
                while (list[i].list.Count < CNT)
                {
                    string newAddr = App.btc.CreateNewAddress("", list[i].name);
                    if(newAddr.Length < 3)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    list[i].list.Add(newAddr);                    
                    needUpdate = true;
                }
            }

            if(needUpdate)
                UpdateInform();
        }

        public string GetOne(string type)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name != ModType(type))
                    continue;

                string addr = list[i].list[new Random().Next(0, list[i].list.Count - 1)];
                list[i].list.Remove(addr);

                return addr;
            }

            UpdateInform();

            return "";
        }

        public static string ModType(string name)
        {
            if (name == DEFAULT_TYPE) return "";
            return name.Trim();
        }

        void UpdateInform()
        {
            var lines = new List<string>() { "clear" };

            foreach (BTCAddrItems group in list)
            {
                lines.Add((group.name == "" ? "default" : group.name) + ":");
                foreach (string item in group.list)
                {
                    lines.Add(item);
                }
            }

            Program.informer(new InformerMessage(InformerMessageWindow.btcAddresses, lines.ToArray()));
        }
    }
}
