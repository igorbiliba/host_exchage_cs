using host_exchage_cs.Data;
using host_exchage_cs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using host_exchage_cs.Components;
using host_exchage_cs.Host.CORS;
using host_exchage_cs.Components.TicketClient;
using System.Diagnostics;
using host_exchage_cs.Models;
using Newtonsoft.Json;
using host_exchage_cs.Components.TicketClient.Responses;
using System.Globalization;
using host_exchage_cs.Helper;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using host_exchage_cs.Host.Responses;
using System.Configuration;
using host_exchage_cs.Actions;
using System.Windows.Forms;
using System.IO;
using host_exchage_cs.Info;

namespace host_exchage_cs
{
    public class Program
    {
        public static List<KeyValuePair<string, int>> CLIENT_USEDS = new List<KeyValuePair<string, int>>();

        public static void UpdateCLIENT_USEDS()
        {
            var lines = new List<string>() { "clear" };

            foreach (var i in CLIENT_USEDS)
                lines.Add(i.Key + ": " + i);

            Program.informer(new InformerMessage(InformerMessageWindow.clientCntUseds, lines.ToArray()));
        }

        public static InformerDelegate informer = null;
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            InfoForm form = new InfoForm();
            informer = form.OnMessage;
            (new Thread(() => { Application.Run(form); })).Start();


            Console.WriteLine("Loading.......\n");
            App.Init();

            //var emails = App.emailParser.GetLastByClient(App.settings.data.ticket_clients.Last());
            //TicketClientCreateResponse ticketByEmail;
            //CreateResponse createResponseByEmail = EmailParser.Find(emails, "bc1qd7yr2x5ev4nfuurwxzq7r5qtpl84z5zrz4u305", "79060675286", out ticketByEmail);
            //Console.WriteLine(createResponseByEmail.account);
            //Console.ReadKey();

            Console.WriteLine("Init rate list....");
            informer(new InformerMessage(InformerMessageWindow.balances, "Init rate list...."));
            App.LAST_PARSED_RATE = ParseRate();
            Console.WriteLine("complete\n");

            Console.WriteLine("Init list btc addr....");
            informer(new InformerMessage(InformerMessageWindow.btcAddresses, "Init list btc addr...."));
            App.btcAddrStorage.UpdateAddressList();
            Console.WriteLine("complete\n");

            foreach (var hostSettings in App.settings.data.hosts)
            {
                if (hostSettings.use_ssl_and_rpc_auth)
                {
                    App.cert.RegenCert(hostSettings.address);
                    Console.WriteLine("Valid ssl cert: " + App.cert.cert.Verify() + "\n");
                    (new Thread(() => {
                        StartSSLWCFService(hostSettings);
                    })).Start();
                }
                else
                {
                    (new Thread(() => {
                        StartWCFService(hostSettings);
                    })).Start();
                }
            }            

            (new Thread(BTCUnlock)).Start();
            (new Thread(BalancesChecker)).Start();
            (new Thread(RateUpdater)).Start();
            (new Thread(BTCAddrGenerator)).Start();
            (new Thread(LoadLastEmails)).Start();
        }

        static void LoadLastEmails()
        {
            while (true)
            {
                try
                {
                    App.emailParser.LoadLastEmails(App.settings.data.parse_last_emails);
                }
                catch (Exception ex) { }
                Thread.Sleep(App.settings.data.interval_parse_emails_sec * 1000);
            }            
        }

        static void BTCUnlock()
        {
            while (true)
            {
                try
                {
                    App.btc.WalletPassphrase(App.settings.data.btc_wallet_password);
                }
                catch (Exception ex) { }
                Thread.Sleep(30000);
            }
        }

        static void BTCAddrGenerator()
        {
            int cnt = App.settings.data.storage_btc_addr_cnt;   

            while (true)
            {
                try
                {
                    App.btcAddrStorage.UpdateAddressList();
                }
                catch (Exception ex) { }
                Thread.Sleep(1000);
            }
        }

        static void BalancesChecker()
        {
            while (true)
            {
                try
                {
                    App.btc.WalletPassphrase(App.settings.data.btc_wallet_password);

                    long timeFrom = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() - (App.settings.data.canceled_min * 60);

                    BtcAddressModel         targetModel = new BtcAddressModel(App.db);
                    BtcAddressForwardModel forwardModel = new BtcAddressForwardModel(App.db);

                    List<string> addresses = targetModel.GetEmptyBtcAddresses(timeFrom);

                    foreach (string address in addresses)
                    {
                        double balance = App.btc.GetBalance(address);
                        if (balance <= 0) continue;

                        targetModel.UpdateIncomeBalance(address, balance);
                        CheckResponse incomeChecker = new CheckResponse(targetModel.getByHash(address), App.settings);

                        try
                        {
                            if (incomeChecker.status == CheckResponse.PAID)
                                SendToForwardAndUpdateStatus(address, balance, forwardModel);
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception ex) { }
                Thread.Sleep(1000);
            }
        }

        static bool SendToForwardAndUpdateStatus(string address, double balance, BtcAddressForwardModel forwardModel)
        {
            var currentForward = forwardModel.GetByBtcFrom(address);

            if (currentForward        == null)                                return false;
            if (currentForward.status == BtcAddressForwardEntity.STATUS_SENT) return false;

            double forwardAmount = balance;
            if (currentForward.dempingpercent > 0) //вычесть dempingpercent
            {
                forwardAmount -= (balance / 100) * currentForward.dempingpercent;
            }
            else if(currentForward.GetForwardAmount() > 0) //forwardint и forwardfraction - точное значение, что нужно отослать
            {
                forwardAmount = currentForward.GetForwardAmount();
            }

            var sendStatus = App.btc.Send(currentForward.btc_to, (decimal)forwardAmount);
            if (sendStatus.IndexOf("txid") == -1) return false;
            forwardModel.UpdateStatus(int.Parse(currentForward.id), BtcAddressForwardEntity.STATUS_SENT);

            return true;
        }

        static RateParsedData ParseRate(int cntTry = 3)
        {
            while (--cntTry > 0)
            {
                try
                {
                    informer(new InformerMessage(InformerMessageWindow.balances, new string[] { "clear", DateTime.Now.ToString() }));
                    
                    RateParsedData rate = new MinimalRateAction().GetMinimal();
                    
                    if (rate.clients.Count > 0) return rate;
                    Thread.Sleep(1000);
                }
                catch (Exception) { }
            }

            return new RateParsedData()
            {
                balance = 0,
                rate    = 0,
                client  = new TicketClientSettingsData(),
                clients = new List<RateParsedData>()
            };
        }

        static void RateUpdater()
        {
            while(true)
            {
                try
                {
                    App.LAST_PARSED_RATE = ParseRate();
                }
                catch (Exception) { }
                Thread.Sleep(App.settings.data.cource_update_sec * 1000);
            }
        }

        static void StartSSLWCFService(HostSettingsDataItem hostSettings)
        {   
            ServiceHost host = new ServiceHost(typeof(WCF_WITH_SSL), new Uri(hostSettings.address));

            foreach (ServiceEndpoint ep in host.Description.Endpoints)
                ep.Behaviors.Add(new BehaviorAttribute());

            host.Credentials.ServiceCertificate.SetCertificate(
              StoreLocation.CurrentUser,
              StoreName.Root,
              X509FindType.FindByIssuerName,
              Cert.USER
            );
            host.Open();

            string address = host.Description.Endpoints[0].ListenUri.AbsoluteUri;
            Console.WriteLine("Https service is opened");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            host.Close();
        }

        static void StartWCFService(HostSettingsDataItem hostSettings)
        {
            ServiceHost host = new ServiceHost(typeof(WCF_NO_SSL), new Uri(hostSettings.address));

            foreach (ServiceEndpoint ep in host.Description.Endpoints)
            {
                ep.Behaviors.Add(new BehaviorAttribute());
                ep.Binding.OpenTimeout    = new TimeSpan(0, 10, 0);
                ep.Binding.CloseTimeout   = new TimeSpan(0, 10, 0);
                ep.Binding.SendTimeout    = new TimeSpan(0, 10, 0);
                ep.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            }

            host.Open();

            Console.WriteLine("Http service is opened");
            Console.WriteLine("Press ctrl+c for close");
        }
    }
}
