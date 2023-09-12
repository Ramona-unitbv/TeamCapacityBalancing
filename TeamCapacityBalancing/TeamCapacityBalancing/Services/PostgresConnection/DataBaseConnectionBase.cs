using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamCapacityBalancing.Services.Postgres_connection;

namespace TeamCapacityBalancing.Services.PostgresConnection
{
    public abstract class DataBaseConnectionBase
    {
        abstract public void ConnectToJira();
        abstract public void DisconnectFromJira();
        abstract public bool RunQuery(DBQuery query);
        abstract public DBQueryItemBase? NextRow();
    }

    public static class DataBaseConnectionBaseFactory
    {
        public static DataBaseConnectionBase GetMeTheRightConnection()
        {   
            return new DataBaseConnectionSQL();
        }
    }
}
