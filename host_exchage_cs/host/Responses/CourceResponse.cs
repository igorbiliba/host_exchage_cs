using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host.Responses
{
    [DataContractAttribute]
    public class CourceResponse
    {
        [DataMember]
        public double cource;
        [DataMember]
        public double balance;
        [DataMember]
        public string client;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
