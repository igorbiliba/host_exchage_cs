using host_exchage_cs.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host_exchage_cs.Components
{
    public class DB {
        public static string DB_PATH {
            get { return Environment.CurrentDirectory + "\\db.db3"; }
        }

        public static void RemoveDB()
        {
            if (File.Exists(DB_PATH))
                File.Delete(DB_PATH);
        }

        public DB() {
            if (!File.Exists(DB_PATH)) Init();
            else OpenConnection();

            Migrate();
        }

        SQLiteFactory factory = null;
        SQLiteConnection connection = null;
        void OpenConnection() {
            if (factory != null || connection != null) return;

            factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            connection = (SQLiteConnection)factory.CreateConnection();
            connection.ConnectionString = "Data Source = " + DB_PATH;
            connection.Open();
        }

        void Init() {
            SQLiteConnection.CreateFile(DB_PATH);
            OpenConnection();
            Migrate();
        }

        public void Migrate()
        {
            new BtcAddressModel(this).MigrateUp();
            new BtcAddressForwardModel(this).MigrateUp();
            new LogRequestModel(this).MigrateUp();
        }

        public void Execute(string sql, SQLiteParameter[] parameters = null) {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = sql;
                if (parameters != null) command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }            
        }

        public long ExecuteWithPk(string sql, SQLiteParameter[] parameters = null)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = sql + " select last_insert_rowid();";
                if (parameters != null) command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;

                return (long)command.ExecuteScalar();
            }
        }

        public string One(string sql, SQLiteParameter[] parameters = null)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = sql;
                if (parameters != null) command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return reader.GetValue(0).ToString();
                    }
                }
            }

            return null;
        }

        public DbDataReader All(string sql, SQLiteParameter[] parameters = null)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = sql;
                if (parameters != null) command.Parameters.AddRange(parameters);
                command.CommandType = CommandType.Text;

                DbDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return reader;
                }
            }

            return null;
        }
    }
}
