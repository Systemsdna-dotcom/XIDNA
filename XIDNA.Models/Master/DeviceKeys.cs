using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class DeviceKeys
    {
        [Key]
        public int DeviceKeysId { get; set; }
        public string DeviceKeyType { get; set; }
        public string KeyDevice { get; set; }
        public int LoginId { get; set; }
    }
}
