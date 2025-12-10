using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    [Table("XIQSScript_T")]
    public class XIQSScripts
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Select Script")]
        public long FKiScriptID { get; set; }
        [Required(ErrorMessage = "Select QuestionSet")]
        public int FKiQSDefinitionID { get; set; }
        [Required(ErrorMessage = "Select Step")]
        public int FKiStepDefinitionID { get; set; }
        public int FKiSectionDefinitionID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlScripts { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQuestionSets { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSStteps { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlSections { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
    }
}