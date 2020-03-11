using host_exchage_cs.Components;
using host_exchage_cs.Components.TicketClient.Responses;
using host_exchage_cs.Data;
using host_exchage_cs.Helper;
using host_exchage_cs.Host.Responses;
using host_exchage_cs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Host
{
    public class WCF_WITH_SSL : IWCF
    {
        public System.IO.Stream Check(string hash)
        {
            try
            {
                if (!App.credentials.IsAllow("check"))
                    return null;

                BtcAddressEntity model = new BtcAddressModel(App.db).getByHash(hash);
                if (model == null) return null;

                string result = new CheckResponse(model, App.settings).toJson();

                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                return new MemoryStream(resultBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public System.IO.Stream Cource()
        {
            int maxCntTry = 60;

            while(--maxCntTry > 0)
            {
                try
                {
                    if (!App.credentials.IsAllow("cource"))
                        return null;

                    string result = "";

                    result = new CourceResponse()
                    {
                        balance = App.LAST_PARSED_RATE.balance,
                        cource = App.LAST_PARSED_RATE.rate,
                        client = App.LAST_PARSED_RATE.GetClientNameOnPath()
                    }.toJson();

                    byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                    return new MemoryStream(resultBytes);
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                }
            }

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            return new MemoryStream(
                Encoding.UTF8.GetBytes(
                    new CourceResponse() {  balance = 0,
                                            cource  = 0,
                                            client  = "false" }.toJson()
                )
            );
        }

        public System.IO.Stream Cources()
        {
            //try
            //{
            //    if (!App.credentials.IsAllow("cources"))
            //        return null;

            //    string result = "";

            //    List<RateParsedData> list = new List<RateParsedData>();
            //    foreach (var client in App.settings.data.ticket_clients)
            //    {
            //        var bullet = new RateParsedData()
            //        {
            //            client = client
            //        };
            //        bullet.ParseOnClient(6000);

            //        result += new CourceResponse()
            //        {
            //            balance         = bullet.balance,
            //            cource          = bullet.rate,
            //            client          = bullet.GetClientNameOnPath()
            //        }.toJson() + ", ";
            //    }

            //    byte[] resultBytes = Encoding.UTF8.GetBytes("["+ result + "]");
            //    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
            //    return new MemoryStream(resultBytes);
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}

            return null;
        }

        long Now() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

        public System.IO.Stream Create(double rubAmount, string phoneFrom, string forwardtobtc = null, int dempingpercent = 0, int forwardint = 0, string forwardfraction = "0")
        {
            long startTime = Now();

            try
            {
                if (!App.credentials.IsAllow("create")) return null;

                string requestStr = @"{ rubAmount : '@rubAmount', phoneFrom : '@phoneFrom', forwardtobtc : '@forwardtobtc', dempingpercent : '@dempingpercent', forwardint : '@forwardint', forwardfraction : '@forwardfraction' }"
                    .Replace("@rubAmount",       rubAmount.ToString())
                    .Replace("@phoneFrom",       phoneFrom)
                    .Replace("@forwardtobtc",    forwardtobtc)
                    .Replace("@dempingpercent",  dempingpercent.ToString())
                    .Replace("@forwardint",      forwardint.ToString())
                    .Replace("@forwardfraction", forwardfraction);

                long requestId = (new LogRequestModel(App.db))
                    .SetRequest("create: " + rubAmount.ToString(), requestStr);

                //ждем курсов
                int waitRateCnt = 10;
                while (App.LAST_PARSED_RATE == null)
                {
                    Thread.Sleep(1000);
                    if (waitRateCnt-- < 0) return null;
                }

                //все доступные клиенты на случай неудачи
                for (int clientId = 0; clientId < App.LAST_PARSED_RATE.clients.Count; clientId++)
                {
                    //max_repeat_on_fault кол-во попыток
                    int cntTryOnClient = App.LAST_PARSED_RATE.client.max_repeat_on_fault == -1 ? 1 : App.LAST_PARSED_RATE.client.max_repeat_on_fault;                    

                    for (int i = 0; i < cntTryOnClient; i++) {
                        string addresstype = App.LAST_PARSED_RATE.client.GetTypeBtcAddress();
                        string btcAddr = App.btcAddrStorage.GetOne(addresstype);

                        (new BtcAddressForwardModel(App.db))
                            .Create(btcAddr, forwardtobtc, dempingpercent, forwardint, forwardfraction);

                        CreateResponse reseponse = null;
                        try
                        {
                            bool isExpire;
                            reseponse = _create(out isExpire, startTime, btcAddr, rubAmount, PhoneHelper.PhoneReplacer(phoneFrom), forwardtobtc, dempingpercent, forwardint, forwardfraction);
                            if(isExpire)
                                return null;
                        }
                        catch (Exception) { reseponse = null; }

                        try {
                            string jsonResponse = "";
                            //не пытаемся получить результат, парсим email
                            if (App.LAST_PARSED_RATE.client.max_repeat_on_fault == -1)
                            {
                                jsonResponse = _getResultByEmail(requestId, startTime, btcAddr, rubAmount, phoneFrom, forwardtobtc, dempingpercent, forwardint, forwardfraction);                                                                
                            }
                            else
                            {
                                //если ответ неудачный, повторяем попытку
                                if (reseponse == null || !reseponse.IsValide()) continue;

                                reseponse.client = App.LAST_PARSED_RATE.ToShort();
                                reseponse.other = new List<KeyValuePair<string, string>> {
                                    new KeyValuePair<string, string>("type_get", "simple")
                                }.ToArray();

                                jsonResponse = reseponse.toJson();
                                (new LogRequestModel(App.db))
                                    .SetResponse(requestId, jsonResponse);
                            }

                            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                            return new MemoryStream(Encoding.UTF8.GetBytes(jsonResponse));
                        } catch (Exception ex) { }

                        if (App.LAST_PARSED_RATE.client.max_repeat_on_fault == -1)
                            return null;
                    }

                    if(!App.LAST_PARSED_RATE.client.allow_switch_to_next_client_on_fault)
                    {
                        return null;
                    }

                    //все попытки завершились неудачей, переключим на следующий клиент
                    App.LAST_PARSED_RATE.Switch();
                }
            }
            catch (Exception) { }

            return null;
        }

        //парсим email, ищем заявку там
        string _getResultByEmail(long requestId, long startTime, string btcAddr, double rubAmount, string phoneFrom, string forwardtobtc, int dempingpercent, int forwardint, string forwardfraction)
        {
            //в течении следующих time_check_email_sec проверяем email
            long maxTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + App.settings.data.time_check_email_sec;
            while (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < maxTime)
            {
                var emails = App.emailParser.GetLastByClient(App.LAST_PARSED_RATE.client);

                TicketClientCreateResponse ticketByEmail = null;
                CreateResponse createResponseByEmail = EmailParser.Find(emails, btcAddr, phoneFrom, out ticketByEmail);
                if (createResponseByEmail == null) //еще не пришло, ждем
                {
                    Thread.Sleep(500);
                    continue;
                }

                try
                {
                    if (forwardtobtc != null)
                        (new BtcAddressForwardModel(App.db))
                            .Create(btcAddr, forwardtobtc, dempingpercent, forwardint, forwardfraction);
                } catch (Exception e) { }

                try {
                    (new BtcAddressModel(App.db)).Create(   startTime,
                                                            btcAddr,
                                                            ticketByEmail.account, //phone to
                                                            ticketByEmail.comment,
                                                            ticketByEmail.btc_amount,
                                                            App.LAST_PARSED_RATE.GetClientNameOnPath(),
                                                            phoneFrom,
                                                            ticketByEmail.ip,
                                                            ticketByEmail.email );
                } catch (Exception e) { }

                createResponseByEmail.other = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("type_get", "complicated")
                }.ToArray();

                try {
                    (new LogRequestModel(App.db)).SetResponse(requestId, createResponseByEmail.toJson());
                } catch (Exception e) { }

                return createResponseByEmail.toJson();
            }

            return null; //после парсинга не даем переключаться на следующий клиент, всегда выходим
        }

        CreateResponse _create(out bool isExpire, long startTime, string btcAddr, double rubAmount, string phoneFrom, string forwardtobtc = null, int dempingpercent = 0, int forwardint = 0, string forwardfraction = "0")
        {
            isExpire = false;
            string hash = "";
            try
            {
                if (btcAddr == "" || btcAddr == null) return null;
                TicketClientCreateResponse ticket = App.ticketClient.Create(rubAmount, phoneFrom, btcAddr, App.LAST_PARSED_RATE.client.max_execute_time_sec);

                hash = (new BtcAddressModel(App.db))
                    .Create(
                        startTime,
                        btcAddr,
                        ticket.account, //phone to
                        ticket.comment,
                        ticket.btc_amount,
                        App.LAST_PARSED_RATE.GetClientNameOnPath(),
                        phoneFrom,
                        ticket.ip,
                        ticket.email
                    );

                if (forwardtobtc != null)
                {
                    (new BtcAddressForwardModel(App.db))
                        .Create(btcAddr, forwardtobtc, dempingpercent, forwardint, forwardfraction);
                }

                return new CreateResponse()
                {
                    account = ticket.account,
                    comment = ticket.comment,
                    hash = hash,
                    btc_amount = ticket.btc_amount.ToString()
                };
            }
            catch (Exception e)
            {
                if (e.Message == "expire")
                    isExpire = true;
            }

            if(hash != "" && hash != null && hash != String.Empty)
                (new BtcAddressModel(App.db)).Remove(hash);

            return null;
        }

        public System.IO.Stream BtcValidate(string address)
        {
            try
            {
                if (!App.credentials.IsAllow("address"))
                    return null;

                var valid = App.btc.ValidateAddress(address);
                string result = App.btc.ValidateAddress(address);

                byte[] resultBytes = Encoding.UTF8.GetBytes(result);
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                return new MemoryStream(resultBytes);
            }
            catch (Exception) { }

            return null;
        }

        public System.IO.Stream Info()
        {
            try
            {
                if (!App.credentials.IsAllow("info"))
                    return null;

                string phone = "+790";
                for (int i = 0; i < 8; i++)
                    phone += new Random()
                                .Next(0, 9)
                                .ToString();

                string addresstype = App.LAST_PARSED_RATE.client.GetTypeBtcAddress();

                var list = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("client",          App.LAST_PARSED_RATE.GetClientNameOnPath()),
                    new KeyValuePair<string, string>("btc_addresstype", App.LAST_PARSED_RATE.client.GetTypeBtcAddress()),
                    new KeyValuePair<string, string>("btc_address",     App.btcAddrStorage.GetOne(addresstype))//App.btc.CreateNewAddress(phone, addresstype)),
                };

                byte[] resultBytes = Encoding.UTF8.GetBytes(String.Join(", ", list));
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";
                return new MemoryStream(resultBytes);
            }
            catch (Exception) { }

            return null;
        }
    }
}