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
    public class XIDocs
    {
        [Key]
        public int ID { get; set; }
        public string FileName { get; set; }
        public int FKiDocType { get; set; }
        public string SubDirectoryPath { get; set; }
        public int iSystemgeneratedorUploaded { get; set; }
        public string sFullPath { get; set; }
        public int iCreatedBy { get; set; }
        public DateTime dCreatedTime { get; set; }
        public string sCreatedBySYSID { get; set; }
        public int iUpdatedBy { get; set; }
        public DateTime dUpdatedTime { get; set; }
        public string sUpdatedBySYSID { get; set; }
        public int FKiACPolicyID { get; set; }
        public int FKiUserID { get; set; }
    }
}

