using host_exchage_cs.Components.TicketClient.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Data
{
    public class TestData
    {
        public TicketClientCreateResponse ticket        = null;
        public string                     btc_address   = null;
        public double                     btc_income    = -1;
        public string                     fake_send_btc = null;
        
        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }

    public class Test {
        public TestData data = new TestData();

        string FILE {
            get { return Environment.CurrentDirectory + "\\Test.json"; }
        }

        public Test() => this.LoadSettings();

        void LoadSettings() {
            if (!File.Exists(FILE)) return;

            string jsonString = System.IO.File.ReadAllText(FILE);
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<TestData>(jsonString);
        }
    }
}
