using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.ViewModels
{
    public class BOAttributeVIewModel
    {
        public int BOID { get; set; }
        public int BOGroupId { get; set; }
        [Required(ErrorMessage = "Please Choose Atleast One Field")]
        public List<ListBoxItems> AvailableFields { get; set; }
        public string Name { get; set; }
        //[Required(ErrorMessage="Enter Group Name")]
        //[Remote("IsExistsGroup", "BusinessObjects", AdditionalFields = "BOID, Type, OldName", HttpMethod = "POST", ErrorMessage = "Group Name already exists. Please enter a different Name.")]
        public string GroupName { get; set; }
        public string BOName { get; set; }
        public bool IsMultiColumnGroup { get; set; }
        public List<ListBoxItems> AssignedFields { get; set; }
        public string Type { get; set; }
        public string OldName { get; set; }
    }

    public class ListBoxItems{
        public string ID {get;set;}
        public string FieldName {get;set;}
    }
}
