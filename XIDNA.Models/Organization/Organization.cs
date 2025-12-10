using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
   public class Organizations
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public int NoOfUsers { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string Website { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public String CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string DatabaseType { get; set; }
        public int ThemeID { get; set; }
        [NotMapped]
        public string Todo { get; set; }
        [NotMapped]
        public int ConID { get; set; }
        [NotMapped]
        public string ConName { get; set; }
        [NotMapped]
        public string ConPhone { get; set; }
        [NotMapped]
        public int ConOrganizationID { get; set; }
        [NotMapped]
        public string ConEmail { get; set; }
        [NotMapped]
        public string ConAddress { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public List<cAggregations> AggregationsList { get; set; }

    }
}


