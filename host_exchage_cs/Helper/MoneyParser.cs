using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Helper
{
    public class MoneyParser
    {
        public static double ParseString(string str)
        {
            double val = 0;

            try
            { val = double.Parse(str.Replace(',', '.')); }
            catch (Exception ex) { val = 0; }

            if (val > 0) return val;
            
            try
            { val = double.Parse(str.Replace('.', ',')); }
            catch (Exception ex) { val = 0; }

            return val < 0 ? 0 : val;
        }
    }
}
