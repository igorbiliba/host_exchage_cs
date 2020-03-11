using host_exchage_cs.Components;
using host_exchage_cs.Helper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace host_exchage_cs.Models
{
    public class BtcAddressForwardEntity
    {
        public const int STATUS_WAIT = 0;
        public const int STATUS_SENT = 1;

        public string   id;
        public string   btc_from;
        public string   btc_to;
        public int      status;
        public int      dempingpercent;        
        public int      forwardint;
        public string   forwardfraction;
        public int      created_at;
        public int      updated_at;

        public double GetForwardAmount()
        {
            try
            {
                return double.Parse(forwardint.ToString() + "." + forwardfraction);
            }
            catch (Exception) { }

            try
            {
                return double.Parse(forwardint.ToString() + "," + forwardfraction);
            }
            catch (Exception) { }

            return 0;
        }
    }

    public class BtcAddressForwardModel
    {
        const string TABLE_NAME = "btc_address_forward";
        DB db;

        public BtcAddressForwardModel(DB db) {
            this.db = db;
        }

        public void MigrateUp()
        {
            try
            {
                db.Execute("CREATE TABLE " + TABLE_NAME + " (" +
                        "id integer PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                        "btc_from char(512) NOT NULL UNIQUE, " +
                        "btc_to char(512) NOT NULL, " +
                        "status integer NOT NULL DEFAULT 0, " +
                        "dempingpercent integer NOT NULL DEFAULT 0," +
                        "created_at integer NOT NULL, " +
                        "updated_at integer NOT NULL DEFAULT 0);");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD forwardint integer NOT NULL DEFAULT 0;");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD forwardfraction char(8) NOT NULL DEFAULT '0';");
            }
            catch (Exception) { }

            try
            {
                db.Execute("ALTER TABLE " + TABLE_NAME + " " +
                    "ADD create_time integer NOT NULL DEFAULT '0';");
            }
            catch (Exception) { }
        }

        long Now() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

        public bool Create(string btc_from, string btc_to, int dempingpercent, int forwardint, string forwardfraction) {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    SQLiteParameter[] parameters = {
                        new SQLiteParameter("@btc_from",        btc_from),
                        new SQLiteParameter("@btc_to",          btc_to),
                        new SQLiteParameter("@dempingpercent",  dempingpercent),
                        new SQLiteParameter("@forwardint",      forwardint),
                        new SQLiteParameter("@forwardfraction", forwardfraction),
                        new SQLiteParameter("@created_at",      Now()),
                    };

                    db.Execute("INSERT INTO " + TABLE_NAME + " " +
                                "(btc_from, btc_to, dempingpercent, forwardint, forwardfraction, created_at) " +
                                "VALUES " +
                                "(@btc_from, @btc_to, @dempingpercent, @forwardint, @forwardfraction, @created_at);",
                                parameters);

                    return true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(100);
                }
            }

            return false;
        }

        public BtcAddressForwardEntity GetByBtcFrom(string btc_from)
        {
            SQLiteParameter[] parameters = { new SQLiteParameter("@btc_from", btc_from) };
            DbDataReader reader = db.All("SELECT * " +
                                          "FROM " + TABLE_NAME + " " +
                                          "WHERE btc_from LIKE @btc_from " +
                                          "LIMIT 1;", parameters);

            if (reader == null) return null;

            reader.Read();

            try
            {
                return new BtcAddressForwardEntity()
                {
                    id              =           reader.GetValue(0).ToString(),
                    btc_from        =           reader.GetValue(1).ToString(),
                    btc_to          =           reader.GetValue(2).ToString(),
                    status          = int.Parse(reader.GetValue(3).ToString()),
                    dempingpercent  = int.Parse(reader.GetValue(4).ToString()),
                    created_at      = int.Parse(reader.GetValue(5).ToString()),
                    updated_at      = int.Parse(reader.GetValue(6).ToString()),
                    forwardint      = int.Parse(reader.GetValue(7).ToString()),
                    forwardfraction = reader.GetValue(8).ToString(),
                };
            }
            catch (Exception) { }

            return null;
        }

        public bool UpdateStatus(int id, int status)
        {
            try
            {
                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@id",         id),
                    new SQLiteParameter("@status",     status),
                    new SQLiteParameter("@updated_at", Now())
                };

                db.Execute("UPDATE " + TABLE_NAME + " " +
                            "SET " +
                            "status = @status, updated_at = @updated_at " +
                            "WHERE id = @id;", parameters);

                return true;
            }
            catch (Exception ex) { }

            return false;
        }
    }
}
