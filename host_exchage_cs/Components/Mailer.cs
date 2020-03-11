using OpenPop.Pop3;
using System;
using System.Collections.Generic;

namespace host_exchage_cs.Components
{
    public class Mailer
    {
        public List<OpenPop.Mime.Message> GetEmails(int parseLastCntMessages)
        {
            List<OpenPop.Mime.Message> messages = new List<OpenPop.Mime.Message>();

            var client = new Pop3Client();
            client.Connect(App.settings.data.email_host, App.settings.data.email_port, true);
            client.Authenticate(App.settings.data.email_login, App.settings.data.email_password);
            int countAllMessages = client.GetMessageCount();

            for(int i = 0; i < parseLastCntMessages && countAllMessages - i > 0; i++)
            {
                try
                {                    
                    messages.Add(
                        client.GetMessage(countAllMessages - i)
                    );
                }
                catch (Exception) { }
            }
            
            return messages;
        }
    }
}
