using host_exchage_cs.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.Bestchange
{
    public class ParseRatesBestchange
    {
        const string FILE_RATE = "bm_rates.dat";

        public Bestchange bestchange;

        public bool LoadFiles()
        {
            if (!bestchange.DownloadFile())
            {
                DeleteFile();
                return false;
            }

            if (!bestchange.UnzipFile())
            {
                DeleteFile();
                return false;
            }

            if (!File.Exists(bestchange.DIR + "/" + FILE_RATE))
                return false;

            bestchange.rates.LoadFile(bestchange.DIR + "/" + FILE_RATE);
            DeleteFile();

            return true;
        }

        void DeleteFile() {
            if (!File.Exists(bestchange.DIR + "/" + FILE_RATE)) return;

            try
            {
                File.Delete(bestchange.DIR + "/" + FILE_RATE);
            }
            catch (Exception) { }
        }

        public (double, double) GetRateBalance(TicketClientSettingsData client, bool rateWithDemping = true)
        {
            if (!bestchange.rates.FindRow(client.rate_path))
                return (-1, -1);

            bm_ratesData data = bestchange.rates.Get();

            if (data == null)
                return (-1, -1);

            double dempingValue = 0;

            if (rateWithDemping && client.demping_percent > 0)
                dempingValue = data.cource / 100 * client.demping_percent;

            return ( data.cource - dempingValue, data.balance );
        }
    }
}
