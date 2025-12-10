using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    [Table("XIQSLink_T")]
    public class XIQSLinks
    {
        [Key]
        public long ID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public Nullable<Guid> FKiQSDefinitionIDXIGUID { get; set; }
        public int FKiSectionDefinitionID { get; set; }
        public Nullable<Guid> FKiSectionDefinitionIDXIGUID { get; set; }
        public int FKiXILinkID { get; set; }
        public Nullable<Guid> FKiXILinkIDXIGUID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public Nullable<Guid> FKiStepDefinitionIDXIGUID { get; set; }
        public string sCode { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSLinkCodes { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQuestionSets { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSStteps { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlSections { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public int XIDeleted { get; set; }
    }
}
