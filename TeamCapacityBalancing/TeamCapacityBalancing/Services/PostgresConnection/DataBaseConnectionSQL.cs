using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;
using TeamCapacityBalancing.Services.PostgresConnection;

namespace TeamCapacityBalancing.Services.Postgres_connection
{
    public class DataBaseConnectionSQL: DataBaseConnectionBase
    {
        private SqlConnection conn = new SqlConnection("Server= belnspdevsql001; Database= jiradb; Integrated Security=True;");
        private bool connected = false;
        private DataTable? table;
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

            SqlDataAdapter da = new SqlDataAdapter(query.Query, conn);

            //Using Data Table
            table = new DataTable();
            da.Fill(table);
            tableIt = 0;
            return true;
        }

        override public DBQueryItemBase? NextRow()
        {
            if (table?.Rows.Count > tableIt)
                return new DBQueryItemSQL(lastQuery.QuerySchema, table.Rows[tableIt++]);
            else
                return null;
        }

    }
}
