using host_exchage_cs.Data;
using host_exchage_cs.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Components
{
    public class BTC
    {
        ICredentials Credentials;
        Uri Url;

        public BTC(string btc_host, string btc_rpcuser, string btc_rpcpassword)
        {
            if (App.TEST_MODE) return;

            Credentials = new NetworkCredential(btc_rpcuser, btc_rpcpassword);
            Url = new Uri(btc_host);
        }

        public JObject InvokeMethod(string methodName, params object[] a_params) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);

            webRequest.Credentials  = Credentials;
            webRequest.ContentType  = "application/json-rpc";
            webRequest.Method       = "POST";

            JObject joe = new JObject();

            joe["jsonrpc"]  = "1.0";
            joe["id"]       = "1";
            joe["method"]   = methodName;

            if (a_params != null && a_params.Length > 0) {
                JArray props = new JArray();
                foreach (var p in a_params) props.Add(p);
                joe.Add(new JProperty("params", props));
            }

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            using (Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                    }
                }
            }

        }

        public JObject WalletPassphrase(string pswd)
        {
            if (App.test.data != null && App.test.data.btc_income != -1 && App.test.data.btc_address != null && App.test.data.btc_address != "")
                return null;

            return InvokeMethod("walletpassphrase", pswd, 3600) as JObject;
        }

        public string CreateNewAddress(string comment, string addresstype)
        {
            if(App.TEST_MODE)
            {
                if(addresstype == "bech32")
                {
                    var addr = TestBtcAddr.bech32List[0];
                    TestBtcAddr.bech32List.RemoveAt(0);
                    return addr;
                } else
                {
                    var addr = TestBtcAddr.simpleList[0];
                    TestBtcAddr.simpleList.RemoveAt(0);
                    return addr;
                }
            }

            int maxTryCnt = 30;

            while(--maxTryCnt > 0)
            {
                try
                {
                    if (App.test.data != null && App.test.data.btc_address != null && App.test.data.btc_address != "")
                        return App.test.data.btc_address;

                    if (addresstype != "")
                        return InvokeMethod("getnewaddress", PhoneHelper.PhoneReplacer(comment), addresstype)["result"].ToString();

                    return InvokeMethod("getnewaddress", PhoneHelper.PhoneReplacer(comment))["result"].ToString();
                }
                catch (Exception) {
                    Thread.Sleep(1000);
                }
            }

            return "";
        }

        public double GetBalance(string address)
        {
            if (App.TEST_MODE)
                return 0;

            if (App.test.data != null && App.test.data.btc_income != -1 && App.test.data.btc_address == address)
                return App.test.data.btc_income;
            
            return (double)InvokeMethod("getreceivedbyaddress", address, 0)["result"];
        }

        public string ValidateAddress(string address)
        {
            if (App.TEST_MODE)
                return "";

            return InvokeMethod("validateaddress", address)["result"].ToString();
        }

        public string Send(string toAddr, decimal amount)
        {
            if (App.TEST_MODE)
                return "{ txid: \"fake id\" }";

            if (App.test.data != null && App.test.data.fake_send_btc == toAddr)
                return "{ txid: \"fake id\" }";

            InvokeMethod("settxfee", 0.0001).ToString();
            return InvokeMethod("sendtoaddress", toAddr, amount, "", "")["result"].ToString();
        }
    }
}
