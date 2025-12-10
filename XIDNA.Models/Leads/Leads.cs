using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace XIDNA.Models
{
    public class Leads
    {
        [Key]
        public long LeadID { get; set; }
        public BOFields ID { get; set; }
        public BOFields InBoundID { get; set; }
        public BOFields UserID { get; set; }
        public BOFields iTeamID { get; set; }
        public BOFields OrgHeirarchyID { get; set; }
        public BOFields sName { get; set; }
        public BOFields sForeName { get; set; }
        public BOFields sLastName { get; set; }
        public BOFields dDateRenewal { get; set; }
        public BOFields FKiLeadClassID { get; set; }
        public BOFields FKiSourceGroupID { get; set; }
        public BOFields FKiSourceID { get; set; }
        public BOFields sMob { get; set; }
        public BOFields sTel { get; set; }
        public BOFields sEmail { get; set; }
        public BOFields sAddress1 { get; set; }
        public BOFields sAddress2 { get; set; }
        public BOFields sAddress3 { get; set; }
        public BOFields sAddress4 { get; set; }
        public BOFields sAddress5 { get; set; }
        public BOFields sPostCode { get; set; }
        public BOFields sCompany { get; set; }
        public BOFields dDOB { get; set; }
        public BOFields sNotes { get; set; }
        public BOFields sBestTimeToCall { get; set; }
        public BOFields iBestDayToCall { get; set; }
        public BOFields iEMailOptOut { get; set; }
        public BOFields iSMSOptOut { get; set; }
        public BOFields iCallOptOut { get; set; }
        public BOFields FKiOrgID { get; set; }
        public BOFields FKiClientID { get; set; }
        public BOFields AddField1 { get; set; }
        public BOFields AddField2 { get; set; }
        public BOFields AddField3 { get; set; }
        public BOFields AddField4 { get; set; }
        public BOFields AddField5 { get; set; }
        public BOFields AddField6 { get; set; }
        public BOFields AddField7 { get; set; }
        public BOFields AddField8 { get; set; }
        public BOFields AddField9 { get; set; }
        public BOFields AddField10 { get; set; }
        public BOFields AddField11 { get; set; }
        public BOFields AddField12 { get; set; }
        public BOFields AddField13 { get; set; }
        public BOFields AddField14 { get; set; }
        public BOFields AddField15 { get; set; }
        public BOFields AddField16 { get; set; }
        public BOFields AddField17 { get; set; }
        public BOFields AddField18 { get; set; }
        public BOFields AddField19 { get; set; }
        public BOFields AddField20 { get; set; }
        public BOFields XICreatedBy { get; set; }
        public BOFields XICreatedWhen { get; set; }
        public BOFields XIUpdatedBy { get; set; }
        public BOFields XIUpdatedWhen { get; set; }
        public BOFields dImportedOn { get; set; }
        public BOFields iPriority { get; set; }
        public BOFields iFinance { get; set; }
        public BOFields iStatus { get; set; }
        public BOFields BrokerRefID { get; set; }
        public BOFields AssignedTime { get; set; }
        public BOFields iCallCount { get; set; }
        public BOFields dtCallSchedule { get; set; }
        public BOFields iCallbackStatus { get; set; }
        public BOFields sSystemAlert { get; set; }
        public BOFields iOutboundUserID { get; set; }
        public BOFields iSalesUserID { get; set; }
        public BOFields sDairyReason { get; set; }
        public BOFields sCoding { get; set; }
    }
}
