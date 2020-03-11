using host_exchage_cs.Data;
using host_exchage_cs.Helper;
using host_exchage_cs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Host.Responses
{
    [DataContractAttribute]
    public class CheckResponse
    {
        public const string PAID       = "paid";       // - сумма на балансе совпадает с размером суммы что возвращал ticketClient
        public const string CANCELED   = "canceled";   // - баланс кошелька != сумме с ticketClient, Если > 24 часов
        public const string PENDING    = "pending";    // - баланс кошелька 0

        public CheckResponse(BtcAddressEntity model, Settings settings)
        {
            double income_balance = MathHelper.TruncateDecimal(model.income_balance, settings.data.btc_precision);
            double income_balance_with_margin = income_balance + settings.data.margin_income_btc_balance;

            double wait_balance   = MathHelper.TruncateDecimal(model.wait_balance,   settings.data.btc_precision);

            long now         = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            long expiredTime = model.created_at + settings.data.canceled_min * 60;
            
            if (income_balance == 0 && now > expiredTime)
            {
                status = CANCELED;
                return;
            }

            if (income_balance != 0 && income_balance_with_margin < wait_balance)
            {
                status = CANCELED;
                return;
            }

            if (income_balance_with_margin >= wait_balance)
            {
                status = PAID;
                return;
            }

            status = PENDING;
        }

        [DataMember]
        public string status;

        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
