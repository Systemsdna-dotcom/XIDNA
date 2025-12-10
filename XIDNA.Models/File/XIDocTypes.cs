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
    public class XIDocTypes : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string SubDirectory { get; set; }
        public string sSizeInMB { get; set; } 
    }
}
