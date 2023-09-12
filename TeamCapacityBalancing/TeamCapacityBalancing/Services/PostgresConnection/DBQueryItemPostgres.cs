using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services.Postgres_connection;

namespace TeamCapacityBalancing.Services.PostgresConnection
{
    public class DBQueryItemPostgres: DBQueryItemBase
    {
        private NpgsqlDataReader _reader;
        public DBQueryItemPostgres(DBQuerySchema aschema, NpgsqlDataReader reader) :
            base(aschema)
        {
            _reader = reader;
        }

        override public string GetString(string key)
        {
            CheckKey(key);
            return _reader.GetString(_reader.GetOrdinal(key));
        }

        override public int GetInt(string key)
        {
            CheckKey(key);
            return _reader.GetInt32(_reader.GetOrdinal(key));
        }
    }
}
