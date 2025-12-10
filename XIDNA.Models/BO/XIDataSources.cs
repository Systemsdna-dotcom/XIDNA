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
    [Table("XIDataSource_XID_T")]
    public class XIDataSources : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public string sConnectionString { get; set; }
        public int FKiOrgID { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid FKiApplicationIDXIGUID { get; set; }
        public string sType { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlOrgs { get; set; }
        [NotMapped]
        public string AllUserDetails { get; set; }
        public int OrganisationID { get; set; }
    }
}

