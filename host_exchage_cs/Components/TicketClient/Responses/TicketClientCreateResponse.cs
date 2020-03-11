using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.TicketClient.Responses
{
    public class TicketClientCreateResponse
    {
        public string account;
        public string comment;
        public double btc_amount;
        public string ip;
        public string email;        

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
