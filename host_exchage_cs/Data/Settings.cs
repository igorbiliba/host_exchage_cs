using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Data
{
    public class MmailParser
    {
        public string   is_client;
        public string[] code;
        public string[] from;
        public string[] to;
		public string[] btc_amount;
        public string[] btc_address;
    }

    public class TicketClientSettingsData
    {
        public const string IS_GET_FROM_TICKET_CLIENT = "get_from_ticket_client";

        public string      path;
        public double      demping_percent;
        public string      btc_addresstype;
        public int         max_repeat_on_fault;
        public bool        allow_switch_to_next_client_on_fault;
        public string      rate_path;
        public MmailParser email_parser;
        public int         max_execute_time_sec;

        public string GetTypeBtcAddress()
        {
            //если IS_GET_IN_TICKET_CLIENT - нужно узнать тип адреса у тикет клиента
            if (btc_addresstype == IS_GET_FROM_TICKET_CLIENT)
            {
                string bin = Environment.CurrentDirectory + "/" + path;

                string[] listPath = path.Split('/');
                if (listPath.Count() > 0)
                    listPath = listPath.Take(listPath.Count() - 1).ToArray();                
                string dir = Environment.CurrentDirectory + "/" + String.Join("/", listPath) + "/";

                return App
                    .ticketClient
                    .GetTypeBtcAddress(bin, dir)
                    .btc_addresstype;
            }
            
            return btc_addresstype;
        }
    }

    public class HostSettingsDataItem
    {
        public bool   use_ssl_and_rpc_auth;        

        string _address;
        public string address
        {
            get
            {
                if (use_ssl_and_rpc_auth)
                    return "https://" + _address + "/";

                return "http://" + _address + "/";
            }
            set
            {
                _address = value;
            }
        }
    }

    public class SettingsData
    {
        public List<HostSettingsDataItem>       hosts;
        public string                           makecert_path;
        public List<TicketClientSettingsData>   ticket_clients;
        public string                           btc_host;
        public string                           btc_rpcuser;
        public string                           btc_rpcpassword;
        public string                           btc_wallet_password;
        public int                              canceled_min;
        public int                              cource_update_sec;
        public int                              btc_precision;
        public double                           margin_income_btc_balance;                
        public string                           storage_btc_addr_types;
        public int                              storage_btc_addr_cnt;
        public string                           allow_methods;
        public string                           rpc_login;
        public string                           rpc_passwrd;
        public double                           allow_min_balance_on_client;
        public int                              expire_time_rates;
        public long                             time_check_email_sec;
        public string                           email_host;
        public string                           email_login;
        public string                           email_password;
        public int                              email_port;
        public int                              parse_last_emails;
        public int                              interval_parse_emails_sec;

        public List<string> GetAllowMethods()
            => allow_methods
                    .Replace(" ", "")
                    .Split(',')
                    .ToList();        
    }

    public class Settings
    {
        public SettingsData data = new SettingsData();

        string FILE {
            get { return Environment.CurrentDirectory + "\\Settings.json"; }
        }

        public Settings()
        {
            this.CreateIfNotExists();
            this.LoadSettings();
        }

        void CreateIfNotExists()
        {
            if (File.Exists(FILE)) return;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FILE, true))
            {
                file.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
        }

        void LoadSettings()
        {
            string jsonString = System.IO.File.ReadAllText(FILE);
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsData>(jsonString);

            if(App.TEST_MODE)
            {
                data.storage_btc_addr_cnt = 5;
            }
        }
    }
}
