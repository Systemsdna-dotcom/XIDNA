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
    //[Table("XIBOOptionList_T")]
    [Table("XIBOOptionList_T_N")]
    public class BOOptionLists
    {
        [Key]
        public int ID { get; set; }
        public int BOID { get; set; }
        public int BOFieldID { get; set; }
        public string Name { get; set; }
        public string sOptionName { get; set; }
        public string sValues { get; set; }
        public string sOptionCode { get; set; }
        public int StatusTypeID { get; set; }
    }
}
