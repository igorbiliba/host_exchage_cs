using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.Bestchange
{
    public class Bestchange
    {
        WebClient wc = new WebClient();
        public bm_rates rates = new bm_rates();

        const string URL = "http://api.bestchange.ru/info.zip";
        string _FILE = null;
        string FILE
        {
            get
            {
                if (_FILE == null)
                    _FILE = Environment.CurrentDirectory + "\\tmp\\Bestchange.zip";
                
                return _FILE;
            }
        }

        public string DIR
        {
            get
            {
                return FILE + "_extracted";
            }
        }

        public bool DownloadFile()
        {
            if (File.Exists(FILE))
                File.Delete(FILE);

            wc.DownloadFile(URL, FILE);

            return true;
        }

        public bool UnzipFile()
        {
            try
            {
                if (Directory.Exists(DIR))
                    Directory.Delete(DIR, true);

                ZipFile.ExtractToDirectory(FILE, DIR);
                return true;
            }
            catch (Exception) { }

            return false;
        }
    }
}
