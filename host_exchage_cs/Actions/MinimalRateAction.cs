using host_exchage_cs.Components.Bestchange;
using host_exchage_cs.Data;
using host_exchage_cs.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Actions
{
    public class MinimalRateAction
    {
        public RateParsedData GetMinimal()
        {
            ParseRatesBestchange beschangeRate = new ParseRatesBestchange() {
                bestchange = new Bestchange()
            };

            if(!beschangeRate.LoadFiles())
            {
                return new RateParsedData();
            }
            
            RateParsedData data = null;

            List<RateParsedData> list = new List<RateParsedData>();
            foreach (TicketClientSettingsData client in App.settings.data.ticket_clients)
            {
                try
                {
                    //rate, balance
                    (double, double) rateItem = client.rate_path == "on_client" ?
                        host_exchage_cs.Components.TicketClient.TicketClient.GetRate(client) :
                        beschangeRate.GetRateBalance(client, false);
                    
                    var bullet = new RateParsedData() { client = client };                    
                    bullet.rate    = rateItem.Item2 >= App.settings.data.allow_min_balance_on_client ? rateItem.Item1 : 0;
                    bullet.balance = rateItem.Item2 >= App.settings.data.allow_min_balance_on_client ? rateItem.Item2 : 0;

                    Program.informer(new InformerMessage(InformerMessageWindow.balances, bullet.GetClientNameOnPath() + ": " + bullet.rate + ", " + bullet.balance));
                    //Console.WriteLine(bullet.GetClientNameOnPath() + ": " + bullet.rate + ", " + bullet.balance);

                    if (bullet.rate > 0)
                    {
                        list.Add(bullet);
                    }
                }
                catch (Exception ex) {
                    Program.informer(new InformerMessage(InformerMessageWindow.balances, "crash load balance: " + ex.Message));
                    //Console.WriteLine("crash load balance: " + ex.Message);
                }
            }

            double minRate = double.MaxValue;
            foreach (RateParsedData item in list)
            {
                if (item.rate < minRate && item.rate > 0)
                {
                    minRate = item.rate;
                    data = item;
                }
            }

            data.clients = list;
            return data;
        }
    }
}
