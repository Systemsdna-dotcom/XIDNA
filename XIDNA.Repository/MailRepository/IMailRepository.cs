using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IMailRepository
    {
        List<string> SelectFoldersWithIMAP(int ID, int OrgID);
        List<string> sGetEmailSubjects(int ID, string sFolder, int OrgID);
        List<string> GetEmailFullDetails(int ID, int iUID, string sFolder, int OrgID);
        IOServerDetails GetEmailCredentials(int ID, int OrgID);
        List<VMDropDown> GetAllMailIDs(int OrgID, string sDatabase);
        List<List<string>> SaveDetailsToDatabase(int ID, int iUID, string sFolder, int OrgID, int iUserID, string sOrgName, string sDatabase);
        List<List<string>> SaveClientMailToDatabase(int ID, int iUID, string sFolder, int OrgID, int UserID, string sDatabase);
        VMCustomResponse SaveMailExtractStrings(VMMailExtractStrings model, int OrgID, string sDatabase);
        DTResponse MailExtractStringsPopUpGrid(jQueryDataTableParamModel param, int OrgID, string sDatabase);
        DTResponse AppNotificationsGrid(jQueryDataTableParamModel param, int OrgID, string databse);
        DTResponse MailExtractStringsGrid(jQueryDataTableParamModel param, int OrgID, string database);
        MailExtractStrings GetMailExtractStringsRow(int ID, int OrgID, string sDatabase);
        List<VMDropDown> AddEditMailExtractStrings(int OrgID, string sDatabase);
        List<List<string>> InsertAPIDataToInbound(Leads model, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetOrgRoles(int OrgID, string sDatabase);
        VMCustomResponse SaveAppNotification(VMAppNotifications model, int UserID, string database);
        //int SaveNotificationImage(int id, string FileName, string database);
        VMAppNotifications EditAppNotifications(int ID, string database);
        List<VMDropDown> GetUsers(int RoleID, string sDatabase);
        AppNotifications SendNotification(int ID, string database);
    }
}
