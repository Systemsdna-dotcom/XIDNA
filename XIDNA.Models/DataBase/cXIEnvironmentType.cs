using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIEnvironment_T")]
    public class cXIEnvironmentType
    {
        [Key]
        public int ID { get; set; }
        public string EnvironmentType { get; set; }
        public string sSourceConnectionstring { get; set; }
        public string sTargetConnectionstring { get; set; }

    }
}
