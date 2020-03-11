using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host.Responses
{
    [DataContractAttribute]
    public class CreateResponse
    {
        [DataMember]
        public string hash;
        [DataMember]
        public string account;
        [DataMember]
        public string comment;
        [DataMember]
        public string btc_amount;
        [DataMember]
        public List<KeyValuePair<string, string>> client;
        [DataMember]
        public KeyValuePair<string, string>[] other;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);

        public bool IsValide()
        {
            if (hash == String.Empty       || hash == null       || hash == "")       return false;
            if (account == String.Empty    || account == null    || account == "")    return false;
            if (comment == String.Empty    || comment == null    || comment == "")    return false;
            if (btc_amount == String.Empty || btc_amount == null || btc_amount == "") return false;

            return true;
        }
    }
}
