using host_exchage_cs.Components;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Models
{
    public class LogRequestModel
    {
        const string TABLE_NAME = "log_requet";
        DB db;

        public LogRequestModel(DB db)
        {
            this.db = db;
        }

        long Now() => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

        public void MigrateUp()
        {
            try
            {
                db.Execute("CREATE TABLE " + TABLE_NAME + " (" +
                            "id integer PRIMARY KEY AUTOINCREMENT NOT NULL, " +
                            "type char(32) NOT NULL, " +
                            "request char(2048) NOT NULL, " +
                            "response char(2048), " +
                            "created_at integer NOT NULL, " +
                            "updated_at integer NOT NULL DEFAULT 0);");
            }
            catch (Exception) { }
        }

        public long SetRequest(string type, string request)
        {
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@type",        type),
                new SQLiteParameter("@request",     request),
                new SQLiteParameter("@created_at",  Now()),
            };

            return db.ExecuteWithPk("INSERT INTO " + TABLE_NAME + " " +
                        "(type, request, created_at) " +
                        "VALUES " +
                        "(@type, @request, @created_at);",
                        parameters);
        }

        public void SetResponse(long id, string response)
        {
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@id",         id),
                new SQLiteParameter("@response",   response),
                new SQLiteParameter("@updated_at", Now()),
            };

            db.Execute( "UPDATE " + TABLE_NAME + " " +
                        "SET " +
                        "response = @response, updated_at = @updated_at " +
                        "WHERE id = @id;", parameters);
        }
    }
}
