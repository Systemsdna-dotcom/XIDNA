using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    public class Datas
    {
        [Key]
        public int id { get; set; }
        public string sName { get; set; }
        //[RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Please Enter Correct Telephone Number")]
        public string sTel { get; set; }
        //[Required(ErrorMessage = "Please Enter Mobile Number")]
        //[RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Please Enter Correct Mobile Number")]
        public string sMob { get; set; }
        public string sAddress1 { get; set; }
        public string sAddress2 { get; set; }
        public string sAddress3 { get; set; }
        public string sAddress4 { get; set; }
        public string sAddress5 { get; set; }
        public string sPreferredName { get; set; }
        public string sAlert { get; set; }
        //[Required(ErrorMessage = "Please Enter Postcode")]
        //[RegularExpression(@"^(?=\D*\d)(?=[^ ]*[ ])(?=[^a-zA-Z]*[a-zA-Z])[0-9a-zA-Z ]+$", ErrorMessage = "Please Enter Correct Postcode")]
        public string sPostCode { get; set; }
        public string dDOB { get; set; }
        //[Required(ErrorMessage = "Please Enter Email")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email Address")]
        public string sEMail { get; set; }        
        public string sCompany { get; set; }
        public string sLifecycle { get; set; }
        public string sSalesOpNotes { get; set; }
        public string sIntroducerCode { get; set; }
        public string sAddress { get; set; }
        //[Required(ErrorMessage = "Please Select LeadClass")]
        public int FKiLeadClassID { get; set; }
        public int FKiClientID { get; set; }
        //[Required(ErrorMessage = "Please Select Source")]
        public int FKiSourceID { get; set; }
        //[Required(ErrorMessage = "Please Enter SourceGroup")]
        public int FKiSourceGroupID { get; set; }
        public int FKiCombinedStatusID { get; set; }
        public int refCategoryID { get; set; }
        public string FKsPickUpID { get; set; }
        public string FKsUWID { get; set; }
        public Nullable<DateTime> dtLastCombinedStatus { get; set; }
        public string sNotes { get; set; }
        //[Required(ErrorMessage = "Please Enter Renewal Date")]
        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string dDateRenewal { get; set; }
        public Nullable<DateTime> dtCallSchedule { get; set; }
        public Nullable<double> rBestQuote { get; set; }
        public Nullable<int> iBestDayToCall { get; set; }
        //[Required(ErrorMessage = "Please Enter EmailOptOut")]
        public Nullable<int> iEMailOptOut { get; set; }
        //[Required(ErrorMessage = "Please Enter SMSOptOut")]
        public Nullable<int> iSMSOptOut { get; set; }
        //[Required(ErrorMessage = "Please Enter CallOptOut")]
        public Nullable<int> iCallOptOut { get; set; }
        public string sMob2 { get; set; }
        public string sMob3 { get; set; }
        public Nullable<int> iPriorityPoints { get; set; }
        public Nullable<int> iEMailStatus { get; set; }
        public Nullable<int> iMobStatus { get; set; }
        public Nullable<int> iTelStatus { get; set; }
        public Nullable<int> iCallBackStatus { get; set; }        
        public Nullable<DateTime> dNextRenew { get; set; }
        public Nullable<int> refSalesInstruction { get; set; }
        public Nullable<int> refReDiaryReason { get; set; }
        public Nullable<int> iPriority { get; set; }
        public string XICreatedBy { get; set; }
        public Nullable<DateTime> XICreatedWhen { get; set; }
        public string XIUpdatedBy { get; set; }
        public Nullable<DateTime> XIUpdatedWhen { get; set; }
        public int FKiUniqueByTimeID { get; set; }        
        public Nullable<int> iPolicyCount { get; set; }
        public Nullable<int> iStatus { get; set; }
        public Nullable<int> iRenewalDateStatus { get; set; }
        public string sHistory { get; set; }
        public string sExtField1 { get; set; }
        public string sExtRef { get; set;}
        public Nullable<int> iCampaignStatus { get; set; }
        public string sEMail2 { get; set; }
        public string sEMail3 { get; set; }
        public string sEMail4 { get; set; }
        public string sEMail5 { get; set; }
        public Nullable<int> iWith { get; set; }
        public Nullable<int> iCallCount { get; set; }
        public Nullable<DateTime> dtConverted { get; set; }
        public Nullable<DateTime> dtQuoted { get; set; }
        public Nullable<DateTime> dtActioned { get; set; }
        public Nullable<double> rAdmin { get; set; }
        //[Required(ErrorMessage = "Please Enter ForeName")]
        public string sForeName { get; set; }
        //[Required(ErrorMessage = "Please Enter LastName")]
        public string sLastName { get; set; }
        public string sBestTimeToCall { get; set; }
        public Nullable<DateTime> dDateIn { get; set; }
        public string sTitle { get; set; }        
        //[ForeignKey("FKiLeadClassID")]
        //public virtual LeadClass LeadClasses { get; set; }
        //[ForeignKey("FKiClientID")]
        //public virtual Clients Clients { get; set; }
        //[ForeignKey("FKiSourceID")]
        //public virtual LeadSource Sources { get; set; }
        //[ForeignKey("FKiSourceGroupID")]
        //public virtual LeadSourceGroups SourceGroups { get; set; }
        //[ForeignKey("FKiUniqueByTimeID")]
        //public virtual UniqueByTimes UniqueByTimes { get; set; }
        //[ForeignKey("refReDiaryReason")]
        //public virtual refReDiaryReason refReDiaryReasons { get; set; }


        [NotMapped]
        public List<VMDropDown> ClassLeadList { get; set; }
        [NotMapped]
        public List<VMDropDown> SourceGroupList { get; set; }
        [NotMapped]
        public List<VMDropDown> SourceList { get; set; }
        [NotMapped]
        public int TotalCount { get; set; }
        [NotMapped]
        public string MobileNo { get; set; }
        [NotMapped]
        public string HomeTelephoneNo { get; set; }
        [NotMapped]
        public string EmailAddress { get; set; }
        [NotMapped]
        public Nullable<DateTime> DOB { get; set; }
        [NotMapped]
        public string PostCode { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public int ClassID { get; set; }

    }
}
