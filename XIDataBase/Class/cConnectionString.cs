using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace XIDatabase
{
    public class cConnectionString
    {
        public string ConnectionString(string database)
        {
            string DataBaseServer = ConfigurationManager.AppSettings["DataBaseServer"];
            string DataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
            string DataBasePassword = ConfigurationManager.AppSettings["DataBasePassword"];
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DataBaseServer;
            sqlBuilder.InitialCatalog = database;
            sqlBuilder.UserID = DataBaseUser;
            sqlBuilder.Password = DataBasePassword;
            sqlBuilder.MultipleActiveResultSets = true;
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.IntegratedSecurity = true;
            return sqlBuilder.ToString();
        }
    }
}
