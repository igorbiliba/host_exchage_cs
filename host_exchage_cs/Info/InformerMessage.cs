using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Info
{
    public enum InformerMessageWindow {
        balances,
        clientCntUseds,
        btcAddresses,
        emailChecker,
        clientsInRun
    };

    public class InformerMessage
    {
        public InformerMessage(InformerMessageWindow window, string message)
        {
            this.window = window;
            this.messages = new string[] { message };
        }

        public InformerMessage(InformerMessageWindow window, string[] messages)
        {
            this.window = window;
            this.messages = messages;
        }

        public InformerMessageWindow window;
        public string[] messages;
    }
}
