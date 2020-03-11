using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace host_exchage_cs.Host
{
    public class ServiceAuthentication : UserNamePasswordValidator
    {
        public override void Validate(string strUserName, string strPassword)
        {
            if (strUserName == null || strPassword == null)
                throw new ArgumentNullException();

            if (strUserName != App.settings.data.rpc_login || strPassword != App.settings.data.rpc_passwrd)
                throw new SecurityTokenException("Unknown Username or Password");
        }
    }
}
