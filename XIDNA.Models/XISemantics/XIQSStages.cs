using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIQSStageConfig_T")]
    public class cXIQSStages : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public int iStage { get; set; }
        public int iChildStage { get; set; }
        public int iType { get; set; }
        [NotMapped]
        public List<cXIQSStages> SVs { get; set; }
    }
}
