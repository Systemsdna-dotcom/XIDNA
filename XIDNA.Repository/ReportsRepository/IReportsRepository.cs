using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Web;

namespace XIDNA.Repository
{
    public interface IReportsRepository
    {
        List<string> GetDialyIncome();
        List<List<string>> GetTransLeadLife();
        List<List<string>> GetLeadLife();
        List<List<string>> GetCLassAndSource();
        //List<VMDashReports> GetDashboardReports(string database);
        //List<VMDashReports> GetClassSource(string database);
        List<VMDashReports> GetOneClickSummary(int ReportID, string Query, int OrgID, string sDatabase);
        List<UserReports> GetAllDashboardReports(int UserID, string sDatabase);
        string RunScheduler(string sDatabase);
        DTResponse GetSchedulersLogList(jQueryDataTableParamModel model, int OrgID, string sDatabase);
    }
}
