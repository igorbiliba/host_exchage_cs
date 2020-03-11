using host_exchage_cs.Components.TicketClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Data
{
    public class RateParsedData
    {
        public double rate = 0;
        public double balance = 0;
        public TicketClientSettingsData client;
        public List<RateParsedData>     clients;

        public List<KeyValuePair<string, string>> ToShort()
            => new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("rate",    rate.ToString()),
                new KeyValuePair<string, string>("balance", balance.ToString()),
                new KeyValuePair<string, string>("name",    GetClientNameOnPath()),
            };

        public string ToJson()
            => Newtonsoft.Json.JsonConvert.SerializeObject(ToShort());
        
        public void Switch()
        {
            clients.Sort(
                delegate (RateParsedData a, RateParsedData b)
                {
                    return (a.balance - b.balance) > 0 ? 1 : -1;
                }
            );

            int currentId = clients.FindLastIndex(
                el => el.GetClientNameOnPath() == GetClientNameOnPath()
            );
            
            if (currentId > clients.Count() - 1) currentId = 0;

            var current = clients[currentId];
            rate        = current.rate;
            balance     = current.balance;
            client      = current.client;
        }

        string clientBin
        {
            get
            {
                return Environment.CurrentDirectory + "/" + client.path;
            }
        }

        string clientDir
        {
            get
            {
                string[] path = client.path.Split('/');
                if (path.Count() > 0)
                {
                    path = path.Take(path.Count() - 1).ToArray();
                }

                return Environment.CurrentDirectory + "/" + String.Join("/", path) + "/";
            }
        }

        public string GetClientNameOnPath()
            => client
                .path
                .Split('/')
                .Last()
                .Replace(".exe", "");
    }
}
