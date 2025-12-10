using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using XIDNA.Models;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.Configuration;
using System.Web.Configuration;

namespace XIDNA.Repository
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("XIDynawareClientDbContext")
        {
        }
        public DataContext(string database)
            : base(ConnectionString(database))
        {
        }
        private static string ConnectionString(string database)
        {
            string DataBaseName = ConfigurationManager.AppSettings["DataBaseServer"];
            string DataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
            string DataBasePassword = ConfigurationManager.AppSettings["DataBasePassword"];
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DataBaseName;
            sqlBuilder.InitialCatalog = database;
            sqlBuilder.UserID = DataBaseUser;
            sqlBuilder.Password = DataBasePassword;
            sqlBuilder.MultipleActiveResultSets = true;
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.IntegratedSecurity = true;
            return sqlBuilder.ToString();
        }

        //public DbSet<Leads> Leads { get; set; }
        //public DbSet<NewLeads> NewLeads { get; set; }
        public DbSet<Reminders> Reminders { get; set; }
        //public DbSet<Users> Users { get; set; }
        public DbSet<OrganizationReports> OrganizationReports { get; set; }
        public DbSet<UserReports> UserReports { get; set; }
        public DbSet<Datas> Datas { get; set; }
        public DbSet<LeadInbounds> LeadInbounds { get; set; }
        public DbSet<Calls> Calls { get; set; }
        public DbSet<EmailDetails> EmailDetails { get; set; }
        //public DbSet<Popup> Popups { get; set; }
        //public DbSet<Stages> Stages { get; set; }
        public DbSet<LeadConfigurations> LeadConfigurations { get; set; }
        public DbSet<OrganizationContacts> OrganizationContacts { get; set; }
        public DbSet<OrganizationLocations> OrganizationLocations { get; set; }
        public DbSet<OrganizationClasses> OrganizationClasses { get; set; }
        public DbSet<OrganizationSources> OrganizationSources { get; set; }
        public DbSet<OrganizationSubscriptions> OrganizationSubscriptions { get; set; }
        public DbSet<LeadMappings> LeadMappings { get; set; }
        public DbSet<OrganizationSourceFields> OrganizationSourceFields { get; set; }
        public DbSet<LeadActions> LeadActions { get; set; }
        public DbSet<Outbounds> Outbounds { get; set; }
        public DbSet<LeadHistories> LeadHistories { get; set; }
        public DbSet<LeadTransitions> LeadTransitions { get; set; }
        public DbSet<LeadStatus> LeadStatus { get; set; }
        public DbSet<MappedFields> MappedFields { get; set; }
        public DbSet<OrganizationImages> OrganizationImages { get; set; }
        public DbSet<ImportHistories> ImportHistories { get; set; }
        public DbSet<ContentEditors> ContentEditors { get; set; }
        public DbSet<ImportingErrorDetails> ImportingErrorDetails { get; set; }
        public DbSet<OrganizationTeams> OrganizationTeams { get; set; }
        public DbSet<LeadActionMenus> LeadActionMenus { get; set; }
        public DbSet<Stages> Stages { get; set; }
        public DbSet<StagesFlows> StagesFlows { get; set; }
        public DbSet<MailExtractStrings> MailExtractStrings { get; set; }
        public DbSet<SubscriptionColumns> SubscriptionColumns { get; set; }
        public DbSet<LeadClients> LeadClients { get; set; }
        public DbSet<WalletRequests> WalletRequests { get; set; }
        public DbSet<WalletProducts> WalletProducts { get; set; }
        public DbSet<WalletPolicies> WalletPolicies { get; set; }
        public DbSet<WalletQuotes> WalletQuotes { get; set; }
        public DbSet<WalletOrders> WalletOrders { get; set; }
        public DbSet<WalletMessages> WalletMessages { get; set; }
        public DbSet<OrganizationDocuments> OrganizationDocuments { get; set; }
        public DbSet<WalletDocuments> WalletDocuments { get; set; }
        public DbSet<Targets> Targets { get; set; }
        public DbSet<Schedulers> Schedulers { get; set; }
        public DbSet<SchedulersLogs> SchedulersLogs { get; set; }
        public DbSet<HTMLColorCodings> HTMLColorCodings { get; set; }
        public DbSet<AppNotifications> AppNotifications { get; set; }
    }
}
