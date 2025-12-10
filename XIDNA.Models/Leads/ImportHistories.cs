using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class ImportHistories
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int UserID { get; set; }
        public string FileName { get; set; }
        public string OriginalName { get; set; }
        public string FileType { get; set; }
        public DateTime ImportedOn { get; set; }
        public int StatusTypeID { get; set; }
        
    }
}
