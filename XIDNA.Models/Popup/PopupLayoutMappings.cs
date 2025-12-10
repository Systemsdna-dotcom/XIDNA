using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XILayoutMapping_T")]
    public class PopupLayoutMappings : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int PopupLayoutID { get; set; }
        public int XiLinkID { get; set; }
        public int PopupID { get; set; }
        public int PlaceHolderID { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string HTMLCode { get; set; }
        public bool IsValueSet { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public Guid XIGUID { get; set; }
    }
}
