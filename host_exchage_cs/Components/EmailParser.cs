using host_exchage_cs.Components.TicketClient.Responses;
using host_exchage_cs.Data;
using host_exchage_cs.Helper;
using host_exchage_cs.Host.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace host_exchage_cs.Components
{
    public class EmailIdentity
    {
        public string phoneFrom;
        public string btcAddr;
    }

    public class EmailParser
    {
        public Mailer mailer;
        List<OpenPop.Mime.Message> emails = new List<OpenPop.Mime.Message>();

        public void LoadLastEmails(int parseLastCntMessages)
        {
            this.emails = mailer.GetEmails(parseLastCntMessages);
        }

        public List<KeyValuePair<EmailIdentity, TicketClientCreateResponse>> GetLastByClient(TicketClientSettingsData client)
        {
            List<KeyValuePair<EmailIdentity, TicketClientCreateResponse>> list = new List<KeyValuePair<EmailIdentity, TicketClientCreateResponse>>();

            foreach (var email in emails)
            {
                var text = email.ToMailMessage().Body;
                
                if (text.IndexOf(client.email_parser.is_client) == -1) continue;

                string matchCodeRow = Regex.Match(text,         client.email_parser.code[0]).Value;
                string matchCode    = Regex.Match(matchCodeRow, client.email_parser.code[1]).Value;
                
                string matchFromRow = Regex.Match(text,         client.email_parser.from[0]).Value;
                string matchFrom    = Regex.Match(matchFromRow, client.email_parser.from[1]).Value;

                string matchToRow = Regex.Match(text,       client.email_parser.to[0]).Value;
                string matchTo    = Regex.Match(matchToRow, client.email_parser.to[1]).Value;

                string matchBtcAmountRow = Regex.Match(text,              client.email_parser.btc_amount[0]).Value;
                string matchBtcAmount    = Regex.Match(matchBtcAmountRow, client.email_parser.btc_amount[1]).Value;

                string matchBtcAddressRow = Regex.Match(text,               client.email_parser.btc_address[0]).Value;
                string matchBtcAddress    = Regex.Match(matchBtcAddressRow, client.email_parser.btc_address[1]).Value;

                list.Add(new KeyValuePair<EmailIdentity, TicketClientCreateResponse>(
                    new EmailIdentity() {
                        btcAddr   = HTMLHelper.StripHTML(matchBtcAddress).Trim(),
                        phoneFrom = HTMLHelper.StripHTML(matchFrom).Trim()
                    },
                    new TicketClientCreateResponse() {
                        account    = HTMLHelper.StripHTML(matchTo).Trim(),
                        comment    = HTMLHelper.StripHTML(matchCode).Trim(),
                        btc_amount = MoneyParser.ParseString(HTMLHelper.StripHTML(matchBtcAmount)),
                        email      = email.ToMailMessage().To.ToString().Trim()
                    }
                ));
            }

            return list;
        }

        public static CreateResponse Find(List<KeyValuePair<EmailIdentity, TicketClientCreateResponse>> list, string btcAddress, string phoneFrom, out TicketClientCreateResponse ticket)
        {
            ticket     = null;
            phoneFrom  = PhoneHelper.PhoneReplacer(phoneFrom).Trim();
            btcAddress = btcAddress.Trim().ToLower();

            if (!list
                .Exists(el =>
                    (PhoneHelper.PhoneReplacer(el.Key.phoneFrom).Trim()) == phoneFrom
                    &&
                    el.Key.btcAddr.Trim().ToLower() == btcAddress
                )
            ) return null;

            ticket = list
                .Where(el =>
                    (PhoneHelper.PhoneReplacer(el.Key.phoneFrom).Trim()) == phoneFrom
                    &&
                    el.Key.btcAddr.Trim().ToLower() == btcAddress
                )
                .First()
                .Value;

            if (ticket == null) return null;

            return new CreateResponse()
            {
                account     = ticket.account,
                comment     = ticket.comment,
                hash        = btcAddress,
                btc_amount  = ticket.btc_amount.ToString()
            };
        }
    }
}
