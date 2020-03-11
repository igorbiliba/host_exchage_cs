using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Data
{
    public class LogItem
    {
        public string type;
        public string exception;
        public string item;
        public string addresstype;
        public string btcAddr;
        public string phone;
        public string client;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

    public class Log
    {
        string FILE
        {
            get { return Environment.CurrentDirectory + "\\Log.json"; }
        }

        public void Append(LogItem item)
        {
            if (!File.Exists(FILE))
                File.Create(FILE);

            File.AppendAllText(FILE, item.toJson() + ",\n");
        }
    }
}
