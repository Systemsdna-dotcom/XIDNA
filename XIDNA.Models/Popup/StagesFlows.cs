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
    public class StagesFlows
    {
        public int ID { get; set; }
        public int StageID { get; set; }
        public string SubStages { get; set; }
        public int StatusTypeID { get; set; }
        public int OrganizationID { get; set; }
        [NotMapped]
        public List<VMDropDown> Stages { get; set; }
        [NotMapped]
        public List<VMDropDown> StagesList { get; set; }
        [NotMapped]
        public List<string> SStages { get; set; }
        [NotMapped]
        public bool CheckStage { get; set; }
        [NotMapped]
        public string NewStage { get; set; }
    }
}
