using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services.Postgres_connection;
using Npgsql;
using Tmds.DBus;

namespace TeamCapacityBalancing.Services.PostgresConnection
{
    public class DataBaseConnectionPostgres : DataBaseConnectionBase
    {
        private NpgsqlConnection conn = new NpgsqlConnection(DataBaseConnection.GetInstance().GetConnectionString());
        private bool connected = false;
        private NpgsqlDataReader? reader;
        private DBQuery lastQuery;
        int tableIt = 0;

        override public void ConnectToJira()
        {
            try
            {
                conn.Open();
                connected = true;
            }
            catch (Exception exc)
            {
                //MessageBox.Show("Failed to connect to Jira! Error: " + exc.ToString());
            }
        }

        override public void DisconnectFromJira()
        {
            connected = false;
            conn.Close();
        }

        override public bool RunQuery(DBQuery query)
        {
            if (!connected)
                return false;

            lastQuery = query;

            var cmd = new NpgsqlCommand(query.Query, conn);

            reader = cmd.ExecuteReader();
            return true;
        }

        override public DBQueryItemBase? NextRow()
        {
            if (reader != null && reader.Read())
                return new DBQueryItemPostgres(lastQuery.QuerySchema, reader);
            else
                return null;
        }
    }
}
