using host_exchage_cs.Components.TicketClient.Responses;
using host_exchage_cs.Data;
using host_exchage_cs.Info;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.TicketClient
{
    public class TicketClient
    {
        Settings settings;
        public TicketClient(Settings settings)
        {
            this.settings = settings;
        }

        string TICKET_CLIENT_NAME
        {
            get
            {
                return App
                    .LAST_PARSED_RATE
                    .client
                    .path
                    .Split('/')
                    .Last()
                    .Replace(".exe", "");
            }
        }

        string TICKET_CLIENT_BIN {
            get {
                if (App.LAST_PARSED_RATE == null)
                    return null;
                return Environment.CurrentDirectory + "/" + App.LAST_PARSED_RATE.client.path;
            }
        }

        string TICKET_CLIENT_DIR
        {
            get
            {
                if (App.LAST_PARSED_RATE == null)
                    return null;

                string[] path = App.LAST_PARSED_RATE.client.path.Split('/');
                if(path.Count() > 0)
                {
                    path = path.Take(path.Count() - 1).ToArray();
                }
                
                return Environment.CurrentDirectory + "/" + String.Join("/", path) + "/";
            }
        }

        public TypeBtcAddressResponse GetTypeBtcAddress(string bin = "", string dir = "")
        {
            string response = Exec(
                bin == "" ? TICKET_CLIENT_NAME : bin.Split('/').Last().Replace(".exe", ""),
                bin == "" ? TICKET_CLIENT_BIN : bin,
                dir == "" ? TICKET_CLIENT_DIR : dir,
                "--gettypebtcaddress",
                100,
                1000 * 60 * 10 //можно 10 мин морозить процесс
            );

            return JsonConvert.DeserializeObject<TypeBtcAddressResponse>(response);
        }

        public static (double, double) GetRate(TicketClientSettingsData clientData, int expire = 10000)//10 сек на случай зависона
        {
            string bin      = Environment.CurrentDirectory + "/" + clientData.path;
            string[] path   = clientData.path.Split('/');            
            if (path.Count() > 0) path = path.Take(path.Count() - 1).ToArray();
            string dir      = Environment.CurrentDirectory + "/" + String.Join("/", path) + "/";
            string clientName = clientData
                    .path
                    .Split('/')
                    .Last()
                    .Replace(".exe", "");

            string response = Exec(clientName, bin, dir, "--rate", 100, expire);

            if (response == null || response == "expire") return (0, 0);

            RateParsedData model = JsonConvert.DeserializeObject<RateParsedData>(response);

            if (clientData.demping_percent > 0)
                model.rate = model.rate - (model.rate / 100 * clientData.demping_percent);

            return (model.rate, model.balance);
        }

        public TicketClientCreateResponse Create(double rubAmount, string phone, string btcAddr, int maxExecuteTimeSec)
        {
            if(
                App.test.data != null && App.test.data.ticket != null ||
                App.LAST_PARSED_RATE == null
            )
                return JsonConvert.DeserializeObject<TicketClientCreateResponse>(
                    App.test.data.ticket.toJson()
                );

            string arguments
                =   "--create" +
                    " " + rubAmount +
                    " " + phone +
                    " " + btcAddr;

            string response = Exec(
                TICKET_CLIENT_NAME,
                TICKET_CLIENT_BIN,
                TICKET_CLIENT_DIR,
                arguments,
                100,
                1000 * 60 * maxExecuteTimeSec
            );

            if (response == "expire")
                throw new Exception("expire");

            return JsonConvert.DeserializeObject<TicketClientCreateResponse>( response );
        }

        //максимум {expire} на процесс, дальше умирать
        public static string Exec(string clientName, string bin, string dir, string arguments, int delayBeforeNextProcess, int expire)
        {
            if (!Program.CLIENT_USEDS.Exists(el => el.Key == clientName))
                Program.CLIENT_USEDS.Add(new KeyValuePair<string, int>(clientName, 0));

            int clientIdx = Program
                .CLIENT_USEDS
                .FindIndex(el => el.Key == clientName);

            Program.CLIENT_USEDS[clientIdx] = new KeyValuePair<string, int>(
                Program.CLIENT_USEDS[clientIdx].Key,
                Program.CLIENT_USEDS[clientIdx].Value + 1
            );
            Program.UpdateCLIENT_USEDS();
            Program.informer(new InformerMessage(InformerMessageWindow.clientsInRun, clientName + " " + arguments));

            try
            {
                if (delayBeforeNextProcess > 0)
                    Thread.Sleep(delayBeforeNextProcess);

                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName               = bin,
                        WorkingDirectory       = dir,
                        Arguments              = arguments,
                        UseShellExecute        = false,
                        CreateNoWindow         = true,
                        RedirectStandardOutput = true,
                    }
                };
                
                if (process.Start())
                {
                    if (process.WaitForExit(expire))
                    {
                        string output = process.StandardOutput.ReadToEnd();

                        Program.informer(new InformerMessage(InformerMessageWindow.clientsInRun, clientName + ": " + output));

                        return output;
                    }
                    else
                    {
                        Program.informer(new InformerMessage(InformerMessageWindow.clientsInRun, clientName + ": process is expired: " + arguments));
                        return "expire";
                    }
                }
            }
            catch (Exception) { }

            return null;
        }
    }
}
