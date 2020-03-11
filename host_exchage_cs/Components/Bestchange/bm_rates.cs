using host_exchage_cs.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components.Bestchange
{
    public class bm_ratesData
    {
        public double cource;
        public double balance;
    }

    public class bm_rates
    {
        string content = "";
        string row = "";
        public void LoadFile(string path)
        {
            content = File.ReadAllText(path);
        }

        public bool FindRow(string bullet)
        {
            string[] lines = content.Split('\n');
            var rez = lines.Where(el => el.IndexOf(bullet) == 0);

            if (rez.Count() < 1) return false;

            row = rez.First();

            return true;
        }

        public bm_ratesData Get()
        {
            try
            {
                var list = row.Split(';');
                return new bm_ratesData()
                {
                    cource = MoneyParser.ParseString(list[3]),
                    balance = MoneyParser.ParseString(list[5]),
                };
            }
            catch (Exception) { }

            return null;
        }
    }
}
