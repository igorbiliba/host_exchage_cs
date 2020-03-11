using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Helper
{
    public class PhoneHelper
    {
        public static string PhoneReplacer(string phone)
            => phone
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("+", "");
    }
}
