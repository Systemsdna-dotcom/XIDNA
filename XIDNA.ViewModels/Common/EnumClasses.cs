using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public enum BODatatypes
    {
        BIGINT = 10,
        BIT = 20,
        SMALLINT = 30,
        DECIMAL = 40,
        SMALLMONEY = 50,
        INT = 60,
        TINYINT = 70,
        MONEY = 80,
        FLOAT = 90,
        REAL = 100,
        DATE = 110,
        DATETIMEOFFSET = 120,
        DATETIME2 = 130,
        SMALLDATETIME = 140,
        DATETIME = 150,
        TIME = 160,
        CHAR = 170,
        VARCHAR = 180,
        TEXT = 190,
        NCHAR = 200,
        NVARCHAR = 210,
        NTEXT = 220,
        PASSWORD=230,
        XISCRIPT=240,
        UNIQUEIDENTIFIER=250,
        TAGCODE=260
    }

    public enum XIDataTypes
    {
        varchar, number, date, time, OptionList, postcode, email, UKMobile, UKLandline
    }

    public enum EnumRoles
    {
        XISuperAdmin, SuperAdmin, Admin, User, WebUsers, AppAdmin, OrgAdmin, OrgIDE, DeveloperStudio, PFOClient
    }

    public enum EnumDisplayTypes
    {
        KPICircle = 10, PieChart = 20, BarChart = 30, LineChart = 40, ResultList = 50, Summary = 60, Bespoke = 70, ViewRecord = 80, EmailReport = 90, CustomQuery = 100, Grid = 110, Repeater = 120, List = 130
    }

    public enum EnumLocations
    {
        Dashboard = 10, Reports = 20, Search = 30, Inbox = 40, Notifications = 50, QuickSearch = 60, DashboardReports = 70, AppDashboard = 80, Preferences=90
    }

    public enum EnumLeadTables
    {
        Leads, LeadClients, LeadInstances, LeadInbounds, Reports, WalletQuotes, WalletPolicies, OrganizationClasses
    }

    public enum EnumSearchType
    {
        FilterSearch, NaturalSearch
    }

    public enum EnumSemanticsDisplayAs
    {
        EditForm = 10, Sections = 20, Fields = 30, XIComponent = 40, Html = 50, Bespoke = 60, OneClick = 70
    }

    public enum EnumBOTypes
    {
        MasterEntity, Reference, Enum, XISystem, Technical
    }
    public enum EnumPolicyLookupResponses
    {
        Normal, Refer, Decline
    }
    public enum EnumCacheTypes
    {
        None, Application, User
    }

}
