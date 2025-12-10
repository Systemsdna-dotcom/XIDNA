using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMXiLinks
    {
        public int XiLinkID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
        public int FKiComponentID { get; set; }
        public string List { get; set; }
        public List<string> Names { get; set; }
        public List<string> Values { get; set; }
        public List<string> ListNames { get; set; }
        public List<string> ListValues { get; set; }
        public string[] NVPairs { get; set; }
        public string[] LNVPairs { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public List<VMXiLinkNVs> NVs { get; set; }
        public List<VMXiLinkLists> Lists { get; set; }
        public List<VMDropDown> ddlXIComponents { get; set; }
        public int FKiApplicationID { get; set; }
        public string sActive { get; set; }
        public string sType { get; set; }
        public string CreatedBySYSID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        public int OrganisationID { get; set; }
    }

    public class VMXiLinkNVs
    {
        public int XiLinkID { get; set; }
        public int XiLinkListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class VMXiLinkLists
    {
        public int XiLinkID { get; set; }
        public int XiLinkListID { get; set; }
        public string ListName { get; set; }
        public List<VMXiLinkNVs> NVs { get; set; }
    }

}
