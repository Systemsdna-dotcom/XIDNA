using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace XIInfrastructure
{
    public class XIInfraDbContext : DbContext
    {
        private static Dictionary<string, string> coreAppConnections = new Dictionary<string, string>();
        public XIInfraDbContext()
            : base("XIDNADbContext")
        //: base(ConnectionString("XIDNA"))
        {
        }

        public XIInfraDbContext(string database)
            : base(ConnectionString(database))
        {
        }

        public static string ConnectionString(string database)
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
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<XIInfraDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<XIInfraRoles> XIAppRoles { get; set; }
        public DbSet<XIInfraUsers> XIAppUsers { get; set; }
        public DbSet<XIInfraUserRoles> XIAppUserRoles { get; set; }
    }
}