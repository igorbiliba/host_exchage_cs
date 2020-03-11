using host_exchage_cs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components
{
    public class Credentials
    {
        Settings settings;

        public Credentials(Settings settings)
        {
            this.settings = settings;
        }

        public bool IsAllow(string method)
        {
            if (settings.data.allow_methods == "*")
                return true;

            return settings
                .data
                .GetAllowMethods()
                .Contains(method);
        }
    }
}
