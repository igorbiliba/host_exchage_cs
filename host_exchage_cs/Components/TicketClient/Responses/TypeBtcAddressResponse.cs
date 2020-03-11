using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.TicketClient.Responses
{
    public class TypeBtcAddressResponse
    {
        public string btc_addresstype;
        public int target_currency_id;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
