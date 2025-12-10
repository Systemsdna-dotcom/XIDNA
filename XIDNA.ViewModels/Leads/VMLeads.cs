using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeads
    {
        private VMLeads Lead;
        public VMLeads() { }
        public VMLeads(VMLeads Lead)
        {
            // TODO: Complete member initialization
            this.Lead = Lead;
        }
        public long ID { get; set; }
        public int? InBoundID { get; set; }
        public int? UserID { get; set; }
        public int? iTeamID { get; set; }
        public string OrgHeirarchyID { get; set; }
        public string sName { get; set; }
        public string sForeName { get; set; }
        public string sLastName { get; set; }
        public DateTime? dDateRenewal { get; set; }
        public int? FKiLeadClassID { get; set; }
        public int? FKiSourceGroupID { get; set; }
        public int? FKiSourceID { get; set; }
        public string sMob { get; set; }
        public string sTel { get; set; }
        public string sEmail { get; set; }
        public string sAddress1 { get; set; }
        public string sAddress2 { get; set; }
        public string sAddress3 { get; set; }
        public string sAddress4 { get; set; }
        public string sAddress5 { get; set; }
        public string sPostCode { get; set; }
        public string sCompany { get; set; }
        public DateTime? dDOB { get; set; }
        public string sNotes { get; set; }
        public string sBestTimeToCall { get; set; }
        public int? iBestDayToCall { get; set; }
        public int? iEMailOptOut { get; set; }
        public int? iSMSOptOut { get; set; }
        public int? iCallOptOut { get; set; }
        public int? FKiOrgID { get; set; }
        public int? FKiClientID { get; set; }
        public int? AddField1 { get; set; }
        public int? AddField2 { get; set; }
        public int? AddField3 { get; set; }
        public int? AddField4 { get; set; }
        public int? AddField5 { get; set; }
        public string AddField6 { get; set; }
        public string AddField7 { get; set; }
        public string AddField8 { get; set; }
        public string AddField9 { get; set; }
        public string AddField10 { get; set; }
        public DateTime? AddField11 { get; set; }
        public DateTime? AddField12 { get; set; }
        public DateTime? AddField13 { get; set; }
        public DateTime? AddField14 { get; set; }
        public DateTime? AddField15 { get; set; }
        public string AddField16 { get; set; }
        public string AddField17 { get; set; }
        public string AddField18 { get; set; }
        public string AddField19 { get; set; }
        public string AddField20 { get; set; }
        public string XICreatedBy { get; set; }
        public DateTime? XICreatedWhen { get; set; }
        public string XIUpdatedBy { get; set; }
        public DateTime? XIUpdatedWhen { get; set; }
        public DateTime? dImportedOn { get; set; }
        public int? iPriority { get; set; }
        public int? iFinance { get; set; }
        public int? iStatus { get; set; }
        public int? BrokerRefID { get; set; }
        public DateTime? AssignedTime { get; set; }
        public int? iCallCount { get; set; }
        public DateTime? dtCallSchedule { get; set; }
        public int? iCallbackStatus { get; set; }
        public string sSystemAlert { get; set; }
        public int? iOutboundUserID { get; set; }
        public int? iSalesUserID { get; set; }
        public string sDairyReason { get; set; }
        public string sCoding { get; set; }
        public string ConversionStatus { get; set; }
        public string Class { get; set; }
        public int? StatusTypeID { get; set; }
        public string OrganizationName { get; set; }
        public int OrganizationID { get; set; }
        public bool IsReqSent { get; set; }
        public string CallBackStatus { get; set; }
        public int ClassID { get; set; }
        public string Name { get; set; }
    }
}
