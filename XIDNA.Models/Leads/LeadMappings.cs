using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
   public class LeadMappings
    {
        [Key]
        public int ID { get; set; }
        public string SubscriptionID { get; set; }
        public int ClassID { get; set; }
        [NotMapped]
        public string EmailFieldName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        [NotMapped]
        public int OrganizationID { get; set; }
        [NotMapped]
        public List<VMDropDown> Classes { get; set; }
        //[NotMapped]
        //public List<string> Name { get; set; }
        //[NotMapped]
        //public string Field { get; set; }
        //[NotMapped]
        //public string ExtraFields { get; set; }
        //[NotMapped]
        //public string EmailFields { get; set; }
        //[NotMapped]
        //public List<string> FieldTypes { get; set; }
        //[NotMapped]
        //public List<string> ExtraFieldName { get; set; }
        //[NotMapped]
        //public List<string> ExtraFieldTypes { get; set; }
        //[NotMapped]
        //public List<string> ExistingFields { get; set; }
        //[NotMapped]
        //public string DataTypes { get; set; }
        //[NotMapped]
        //public int SourceID { get; set; }
        //[NotMapped]
        //public List<OrgSources> OrgSources { get; set; }
        [NotMapped]
        public List<MappedField> MappedFields { get; set; }
       }
    public class OrgSources
    {
        public string Text { get; set; }
        public string SubID { get; set; }
    }

    public class MappedField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ColumnName { get; set; }
    }
}
