using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace XIDNA.Models
{
    public class ModelDbContext : DbContext
    {
        private static Dictionary<string, string> coreAppConnections = new Dictionary<string, string>();
        public ModelDbContext()
            : base("XIDNADbContext")
        //: base(ConnectionString("XIDNA"))
        {
        }

        public ModelDbContext(string database)
            : base(ConnectionString(database))
        {
        }
        public static string CoreConnectionString(string AppName)
        {
            if (coreAppConnections.ContainsKey(AppName))
            {
                return coreAppConnections[AppName];
            }
            else
            {
                ModelDbContext db = new ModelDbContext();
                string connStr = db.XIApplications.Where(x => x.sApplicationName == AppName).Select(x => x.sConnectionString).FirstOrDefault();
                coreAppConnections.Add(AppName, connStr);
                return coreAppConnections[AppName];
            }
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

        //public static string ConnectionString(string database)
        //{
        //    CoreDbContext CDbContext = new CoreDbContext();
        //    string sqlBuilder = CDbContext.ConnectionString(database);
        //    return sqlBuilder.ToString();
        //}

        private static string ConnectionString(string appname, string database)
        {
            CoreDbContext CDbContext = new CoreDbContext(appname);
            string sqlBuilder = CDbContext.ConnectionString(database);
            return sqlBuilder.ToString();
        }

        public DbSet<cXIApplications> XIApplications { get; set; }
        public DbSet<cXIAppRoles> XIAppRoles { get; set; }
        public DbSet<cXIAppUsers> XIAppUsers { get; set; }
        public DbSet<cXIAppUserRoles> XIAppUserRoles { get; set; }
        public DbSet<BOs> BOs { get; set; }
        public DbSet<BOFields> BOFields { get; set; }
        public DbSet<BOGroupFields> BOGroupFields { get; set; }
        public DbSet<BOOptionLists> BOOptionLists { get; set; }
        public DbSet<BOScripts> BOScripts { get; set; }
        public DbSet<BOScriptResults> BOScriptResults { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<Organizations> Organization { get; set; }
        //public DbSet<OrganizationContacts> OrganizationContacts { get; set; }
        public DbSet<OrganizationReports> OrganizationReports { get; set; }
        public DbSet<UserReports> UserReports { get; set; }
        public DbSet<Types> Types { get; set; }
        public DbSet<BOClassAttributes> BOClassAttributes { get; set; }
        //public DbSet<OrganizationLocations> OrganizationLocations { get; set; }
        public DbSet<OrganizationSetup> OrganizationSetups { get; set; }
        //public DbSet<OrganizationClasses> OrganizationClasses { get; set; }
        public DbSet<Tabs> Tabs { get; set; }
        public DbSet<Tab1Clicks> Tab1Clicks { get; set; }
        public DbSet<Sections> Sections { get; set; }
        public DbSet<BOProperty> BOProperties { get; set; }
        public DbSet<IOServerDetails> IOServerDetails { get; set; }
        public DbSet<MasterTemplates> MasterTemplates { get; set; }
        public DbSet<ImportRules> ImportRules { get; set; }
        public DbSet<Popup> Popups { get; set; }
        public DbSet<cLayouts> Layouts { get; set; }
        public DbSet<PopupLayoutDetails> PopupLayoutDetails { get; set; }
        public DbSet<Stages> Stages { get; set; }
        public DbSet<StagesFlows> StagesFlows { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<DeviceKeys> DeviceKeys { get; set; }
        public DbSet<WalletRequests> WalletRequests { get; set; }
        public DbSet<XiLinks> XiLinks { get; set; }
        public DbSet<XiLinkNVs> XiLinkNVs { get; set; }
        public DbSet<XiLinkLists> XiLinkLists { get; set; }
        public DbSet<PopupLayoutMappings> PopupLayoutMappings { get; set; }
        public DbSet<RightMenuTrees> RightMenuTrees { get; set; }
        public DbSet<UserDialogs> UserDialogs { get; set; }
        public DbSet<ErrorLogs> ErrorLogs { get; set; }
        public DbSet<Dialogs> Dialogs { get; set; }
        public DbSet<UserConfigurations> UserConfigurations { get; set; }
        public DbSet<XiParameters> XiParameters { get; set; }
        public DbSet<XiParameterNVs> XiParameterNVs { get; set; }
        public DbSet<XiParameterLists> XiParameterLists { get; set; }
        public DbSet<XiVisualisations> XiVisualisations { get; set; }
        public DbSet<XiVisualisationNVs> XiVisualisationNVs { get; set; }
        public DbSet<XiVisualisationLists> XiVisualisationLists { get; set; }
        public DbSet<XIFileTypes> XIFileTypes { get; set; }
        public DbSet<XIDocTypes> XIDocTypes { get; set; }
        public DbSet<XIDocs> XIDocs { get; set; }
        public DbSet<cXISemantics> XISemantics { get; set; }
        public DbSet<cXISemanticsSteps> XISemanticsSteps { get; set; }
        public DbSet<cXISemanticsNavigations> XISemanticsNavigations { get; set; }
        public DbSet<cXIComponents> XIComponents { get; set; }
        public DbSet<cXIComponentsNVs> XIComponentsNVs { get; set; }
        public DbSet<cXIComponentParams> XIComponentParams { get; set; }
        public DbSet<cXIComponentTriggers> XIComponentTriggers { get; set; }
        public DbSet<cFieldOrigin> XIFieldOrigin { get; set; }
        public DbSet<cXIFieldOptionList> XIFieldOptionList { get; set; }
        public DbSet<cFieldDefinition> XIFieldDefinition { get; set; }
        public DbSet<cQSDefinition> QSDefinition { get; set; }
        public DbSet<cQSStepDefiniton> QSStepDefiniton { get; set; }
        public DbSet<cQSNavigations> QSNavigations { get; set; }
        public DbSet<cXIDataTypes> XIDataTypes { get; set; }
        public DbSet<cQSInstance> QSInstance { get; set; }
        public DbSet<cQSStepInstance> QSStepInstance { get; set; }
        public DbSet<cFieldInstance> XIFieldInstance { get; set; }
        public DbSet<cUserCookies> UserCookies { get; set; }
        public DbSet<cStepSectionDefinition> StepSectionDefinition { get; set; }
        public DbSet<cStepSectionInstance> StepSectionInstance { get; set; }
        public DbSet<cFullAddress> FullAddress { get; set; }
        public DbSet<XIDataSources> XIDataSources { get; set; }
        public DbSet<XIHelpItem> XIHelpItem { get; set; }
        public DbSet<cQSVisualisations> QSVisualisations { get; set; }
        public DbSet<cAggregations> Aggregations { get; set; }
        public DbSet<cXIStructure> XIStructure { get; set; }
        public DbSet<cXIStructureDetails> XIStructureDetails { get; set; }
        public DbSet<cBOUIDetails> BOUIDetails { get; set; }
        public DbSet<cBOUIDefaults> BOUIDefaults { get; set; }
        public DbSet<PostCodeLookUp> PostCodeLookUp { get; set; }
        public DbSet<cXIUrlMappings> XIUrlMappings { get; set; }
        public DbSet<PaymentGateWay> PaymentGateWay { get; set; }
        public DbSet<XIQSLinks> QSLink { get; set; }
        public DbSet<XI1ClickNVs> XI1ClickNVs { get; set; }
        public DbSet<XI1ClickParameterNDVs> XI1ClickParameterNDVs { get; set; }
        public DbSet<XIQSScripts> XIQSScripts { get; set; }
        public DbSet<ContentEditors> ContentEditors { get; set; }
        public DbSet<XI1ClickLinks> XI1ClickLinks { get; set; }
        public DbSet<XIQSLinkDefinition> QSXiLink { get; set; }
        public DbSet<XI1ClickPermissions> XI1ClickPermissions { get; set; }
        public DbSet<XIInbox> XIInbox { get; set; }
        public DbSet<XISource> XISource { get; set; }
        public DbSet<XIMenuMappings> XIMenuMappings { get; set; }
        public DbSet<XIRoleMenus> XIRoleMenus { get; set; }
        public DbSet<cXIQSStages> XIQSStages { get; set; }
        public DbSet<cSignalRUserconfigsettings> XISignalRUserSettings { get; set; }
        public DbSet<cSignalRQuerySettings> XISignalRDependencyMasterTable { get; set; }
        public DbSet<GoogleReviewData> GoogleReviewsDetails { get; set; }
        public DbSet<cXIEnvironmentType> XIEnvironment { get; set; }
    }
    //public class CoreDbContext : DbContext
    //{
    //    public CoreDbContext()
    //        : base("name=XIDNACoreDbContext")
    //    {
    //    }
    //    public string ConnectionString(string database)
    //    {
    //            CoreDbContext dbcontext = new CoreDbContext();
    //            string sConnectionString = dbcontext.XIDataSources.Where(m => m.sName == database).Select(m => m.sConnectionString).FirstOrDefault();
    //                return sConnectionString;
    //    }
    //    public DbSet<XIDataSources> XIDataSources { get; set; }
    //}
    public class CoreDbContext : DbContext
    {
        public CoreDbContext()
            : base(ModelDbContext.CoreConnectionString("XiCore"))
        {
        }
        public CoreDbContext(string appname)
            : base(ModelDbContext.CoreConnectionString(appname))
        {
        }
        public string ConnectionString(string database)
        {
            CoreDbContext dbcontext = new CoreDbContext();
            string sConnectionString = dbcontext.XIDataSources.Where(m => m.sName.ToLower() == database.ToLower()).Select(m => m.sConnectionString).FirstOrDefault();
            return sConnectionString;
        }
        public DbSet<XIDataSources> XIDataSources { get; set; }
    }
}
