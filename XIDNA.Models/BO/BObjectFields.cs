using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.ComponentModel;
namespace XIDNA.Models
{
    public class BObjectFields
    {
        public string name { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string helpid { get; set; }
        public string datatype { get; set; }
        public string password { get; set; }
        public string multiline { get; set; }
        public string length { get; set; }
        public string precision { get; set; }
        public string optional { get; set; }
        public string searchmethod { get; set; }
        public string column { get; set; }
        public string virtualcolumn { get; set; }
        public string snull { get; set; }
        public string slock { get; set; }
        public string scase { get; set; }
        public string outputlength { get; set; }

    } 
}
