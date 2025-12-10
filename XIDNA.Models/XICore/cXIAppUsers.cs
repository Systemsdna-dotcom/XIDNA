using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace XIDNA.Models
{
    [Table("XIAPPUsers_AU_T")]
    public class cXIAppUsers
    {
        [Key]
        public int UserID { get; set; }
        public int FKiApplicationID { get; set; }
        public int FKiOrganisationID { get; set; }
        public string sUserName { get; set; }
        public string sPasswordHash { get; set; }
        public string sDatabaseName { get; set; }
        public string sCoreDatabaseName { get; set; }
        public string sAppName { get; set; }
        public string sLocation { get; set; }
        public string sPhoneNumber { get; set; }
        public string sEmail { get; set; }
        public string sFirstName { get; set; }
        public string sLastName { get; set; }
        public string sCol0 { get; set; }
        public string sRow1 { get; set; }
        public string sRow2 { get; set; }
        public string sRow3 { get; set; }
        public string sRow4 { get; set; }
        public string sRow5 { get; set; }
        public int iReportTo { get; set; }
        public int iPaginationCount { get; set; }
        public string sMenu { get; set; }
        public int iInboxRefreshTime { get; set; }
        public string SecurityStamp { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        [NotMapped]
        public string sHierarchy { get; set; }
        //public string Discriminator { get; set; }
        [NotMapped]
        public bool IsUserExist { get; set; }
        [NotMapped]
        public int StatusTypeID { get; set; }
    }
    [Table("XIAppUserRoles_AUR_T")]
    public class cXIAppUserRoles
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public virtual cXIAppRoles Groups { get; set; }

    }
}