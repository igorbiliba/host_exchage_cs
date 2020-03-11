using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host.Responses
{
    [DataContractAttribute]
    public class ErrorResponse
    {
        [DataMember]
        public string error;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
