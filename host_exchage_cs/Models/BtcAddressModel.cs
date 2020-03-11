using host_exchage_cs.Components;
using host_exchage_cs.Helper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Models
{
    public class BtcAddressEntity
    {
        public string id;
        public string hash;
        public string btc_addr;
        public string phone;
        public string comment;
        public double income_balance;
        public double wait_balance;
        public int    created_at;
        public int    updated_at;
        public string client;
        public string paid_from;
    }

    public class BtcAddressModel
    {
        const string TABLE_NAME = "btc_address";
        DB db;

        public BtcAddressModel(DB db) {
            this.db = db;
        }

        public void MigrateUp()
        {
            try
            {
                db.Execute("CREATE TABLE " + TABLE_NAME + " (" +
                        "id integer PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                        "hash char(64) NOT NULL UNIQUE, " +
                        "btc_addr char(512) NOT NULL UNIQUE, " +
                        "phone char(16) NOT NULL, " +
                        "comment char(16) NOT NULL, " +
                        "income_balance double(10, 5) NOT NULL DEFAULT 0, " +
                        "wait_balance double(10, 5) NOT NULL DEFAULT 0, " +
                        "created_at integer NOT NULL, " +
                        "updated_at integer NOT NULL DEFAULT 0);");
            } catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " "+
                    "ADD client char(16);");
            } catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD paid_from char(16);");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD create_time integer NOT NULL DEFAULT 0;");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME +
                    " ADD ip char(31);");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD email char(63);");
            }
            catch (Exception) { }
        }

        long Now() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

        //string getHash(string phone, string comment) =>
        //    CryptoHelper.ComputeSha256Hash(phone + comment + Now());

        public BtcAddressEntity getByHash(string hash) {
            SQLiteParameter[] parameters = {new SQLiteParameter("@hash", hash)};
            DbDataReader reader = db.All("SELECT * " +
                                          "FROM " + TABLE_NAME + " " +
                                          "WHERE hash LIKE @hash " +
                                          "LIMIT 1;", parameters);

            if (reader == null) return null;

            reader.Read();
            
            try {
                return new BtcAddressEntity() {
                    id              = reader.GetValue(0).ToString(),
                    hash            = reader.GetValue(1).ToString(),
                    btc_addr        = reader.GetValue(2).ToString(),
                    phone           = reader.GetValue(3).ToString(),
                    comment         = reader.GetValue(4).ToString(),
                    income_balance  = MoneyParser.ParseString(reader.GetValue(5).ToString()),
                    wait_balance    = MoneyParser.ParseString(reader.GetValue(6).ToString()),
                    created_at      = int.Parse(reader.GetValue(7).ToString()),
                    updated_at      = int.Parse(reader.GetValue(8).ToString()),
                    client          = reader.GetValue(9).ToString(),
                    paid_from       = reader.GetValue(10).ToString()
    };
            } catch (Exception) { }

            return null;
        }

        public bool UpdateIncomeBalance(string btc_addr, double income_balance) {
            try {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@btc_addr", btc_addr),
                    new SQLiteParameter("@income_balance", income_balance),
                    new SQLiteParameter("@updated_at", Now())
                };

                db.Execute("UPDATE " + TABLE_NAME + " " +
                            "SET " +
                            "income_balance = @income_balance, updated_at = @updated_at " +
                            "WHERE btc_addr = @btc_addr;", parameters);

                return true;
            } catch (Exception) { }

            return false;
        }

        public bool Remove(string hash)
        {
            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@hash", hash)
                };

                db.Execute("DELETE " + TABLE_NAME + " " +
                            "WHERE hash = @hash;", parameters);

                return true;
            }
            catch (Exception) { }

            return false;
        }

        public string Create(long startTime, string btc_addr, string phone, string comment, double wait_balance, string client, string paid_from, string ip, string email) {
            string hash = btc_addr;
            
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@btc_addr", btc_addr),
                new SQLiteParameter("@hash", hash),
                new SQLiteParameter("@phone", phone),
                new SQLiteParameter("@comment", comment),
                new SQLiteParameter("@wait_balance", wait_balance),
                new SQLiteParameter("@created_at", Now()),
                new SQLiteParameter("@client", client),
                new SQLiteParameter("@paid_from", paid_from),
                new SQLiteParameter("@create_time", Now() - startTime),
                new SQLiteParameter("@ip", ip),
                new SQLiteParameter("@email", email)
            };

            db.Execute("INSERT INTO " + TABLE_NAME + " " +
                        "(hash, phone, comment, created_at, btc_addr, wait_balance, client, paid_from, create_time, ip, email) " +
                        "VALUES " +
                        "(@hash, @phone, @comment, @created_at, @btc_addr, @wait_balance, @client, @paid_from, @create_time, @ip, @email);", parameters);

            return hash;
        }

        public List<string> GetEmptyBtcAddresses(long timeFrom)
        {
            List<string> list = new List<string>();

            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@timeFrom", timeFrom)
                };

                DbDataReader reader = db.All(
                    "SELECT btc_addr " +
                    "FROM " + TABLE_NAME + " " +
                    "WHERE income_balance = 0 AND created_at > @timeFrom;", parameters);

                if (reader == null) return list;

                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<string> GetAllBtcAddresses()
        {
            List<string> list = new List<string>();

            try
            {
                DbDataReader reader = db.All(
                    "SELECT btc_addr " +
                    "FROM " + TABLE_NAME + ";");

                if (reader == null) return list;

                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }
            }
            catch (Exception) { }

            return list;
        }
    }
}
