using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.Models
{
    public class XiVisualisations : CommonProperties
    {
        [Key]
        public int XiVisualID { get; set; }
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Remote("IsExistsXiVisualisationsName", "XiLink", AdditionalFields = "XiVisualID", HttpMethod = "Post", ErrorMessage = "VisualisationName already exists!,choose another Name")]
        public string Name { get; set; }
        public string Type { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        [ForeignKey("XiVisualID")]
        public virtual List<XiVisualisationNVs> XiVisualisationNVs { get; set; }
        public virtual List<XiVisualisationLists> XiVisualisationLists { get; set; }
    }
}
