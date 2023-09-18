using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services.Postgres_connection;

namespace TeamCapacityBalancing.Services.PostgresConnection
{
    public class DBQueryItemSQL: DBQueryItemBase
    {
        DataRow row;
        public DBQueryItemSQL(DBQuerySchema aschema, DataRow arow):
            base(aschema)
        {
            row = arow;
        }

        override public string GetString(string key)
        {
            CheckKey(key);
            return row[key] as string;
        }

        override public int GetInt(string key)
        {
            CheckKey(key);
            decimal result = (decimal)row[key];
            return Convert.ToInt32(result);
        }

        override public double GetDouble(string key)
        {
            CheckKey(key);
            decimal result = (decimal)row[key];
            return Convert.ToDouble(result);
        }
    }
}
