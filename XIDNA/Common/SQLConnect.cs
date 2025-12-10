using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace XIDNA.Common
{
    public class SQLConnect
    {
        public static SqlConnection GetOpenConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["XIDynawareClientDbContext"].ConnectionString;
            var connection = new SqlConnection(cs);
            connection.Open();
            return connection;
        }
    }
}