using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIDataBase
{
    public class XIDBBase
    {
        public string sSelectFields { get; set; }
        public string sWhereFields { get; set; }
        public string sGroupFields { get; set; }
        public string sOrderFields { get; set; }
        public string sTableName { get; set; }

    }
}