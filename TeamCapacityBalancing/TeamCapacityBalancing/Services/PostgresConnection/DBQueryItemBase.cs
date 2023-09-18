using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services.Postgres_connection;

namespace TeamCapacityBalancing.Services.PostgresConnection
{
    public abstract class DBQueryItemBase
    {
        protected DBQuerySchema schema;

        public DBQueryItemBase(DBQuerySchema aschema)
        {
            schema = aschema;
        }

        protected void CheckKey(string key)
        {
            if (!schema.Keys.Contains(key))
                throw new Exception("Unknown key in schema!");
        }

        abstract public string GetString(string key);
        abstract public int GetInt(string key);
        abstract public double GetDouble(string key);
    }
}
