using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
 [Table("PostCodeLookUps_T")]
  public  class PostCodeLookUp
    {
      [Key]
      public int ID { get; set; }
      public string sPostCode { get; set; }
      public string sMPostCode { get; set; }
      public string Group { get; set; }
    }
}
