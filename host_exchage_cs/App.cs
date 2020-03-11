using host_exchage_cs.Actions;
using host_exchage_cs.Components;
using host_exchage_cs.Components.Bestchange;
using host_exchage_cs.Components.TicketClient;
using host_exchage_cs.Data;
using host_exchage_cs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs
{
    public class App
    {
        public static bool TEST_MODE = false;

        public static RateParsedData LAST_PARSED_RATE = null;

        public static Settings          settings;
        public static DB                db;
        public static TicketClient      ticketClient;
        public static BTC               btc;
        public static Test              test;
        public static Cert              cert;
        public static Credentials       credentials;
        public static Log               log;
        public static BTCAddrStorage    btcAddrStorage;
        public static EmailParser       emailParser;

        public static void TestInit()
        {
            TEST_MODE = true;

            log = new Log();
            DB.RemoveDB();

            settings     = new Settings();
            ticketClient = new TicketClient(settings);
            db           = new DB();
            
            /**
             * ticket - якобы созданный тикет от нетекса.
             * btc_income - якобы входящее бабло от btc клиента.
             * btc_address - якобы только что сгенерированный адрес от btc клиента.
             * fake_send_btc - будет всегда якобы отправлять бабло на этот btc адрес.
             */
            test = new Test()
            {
                data = new TestData()
                {
                    ticket = new Components.TicketClient.Responses.TicketClientCreateResponse()
                    {
                        account    = "+79060671232",
                        comment    = "#3877525#",
                        btc_amount = 0.8756
                    },
                    btc_income    = 0.8756,
                    btc_address   = "3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG",
                    fake_send_btc = "3J6jjLs8DBpqPZvNoohDzzsRUqzgWyeMfG"
                }
            };

            btc = new BTC("", "", "");

            btcAddrStorage = new BTCAddrStorage() { CNT = settings.data.storage_btc_addr_cnt };
            btcAddrStorage.Init(
                settings
                    .data
                    .storage_btc_addr_types
                    .Replace(" ", "")
                    .Split(',')
            );
            btcAddrStorage.UpdateAddressList();
        }

        public static void Init()
        {
            TestBtcAddr.Init();

            log = new Log();
            test = new Test();
            settings = new Settings();
            credentials = new Credentials(settings);
            db = new DB();
            ticketClient = new TicketClient(settings);
            btc = new BTC(
                settings.data.btc_host,
                settings.data.btc_rpcuser,
                settings.data.btc_rpcpassword
            );
            cert = new Cert();
            btcAddrStorage = new BTCAddrStorage() { CNT = settings.data.storage_btc_addr_cnt };
            btcAddrStorage.Init(
                settings
                    .data
                    .storage_btc_addr_types
                    .Replace(" ", "")
                    .Split(',')
            );
            emailParser = new EmailParser() { mailer = new Mailer() };            
            emailParser.LoadLastEmails(App.settings.data.parse_last_emails);
        }
    }
}
