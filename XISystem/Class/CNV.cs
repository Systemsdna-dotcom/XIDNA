using System.Collections.Generic;
using System.Threading;

namespace XISystem
{
    public class CNV
    {
        public int ID { get; set; }
        private string Name = "";
        private string Value = "";
        private string Type = "";
        private string Label = "";
        private string Context = "";
        private decimal Order;
       
        private List<CNV> SubParams = new List<CNV>();

        public List<CNV> nSubParams
        {
            get
            {
                return SubParams;
            }
            set
            {
                SubParams = value;
            }
        }

        private Dictionary<string, CNV> oNNVs = new Dictionary<string, CNV>();

        public Dictionary<string, CNV> NNVs
        {
            get
            {
                return oNNVs;
            }
            set
            {
                oNNVs = value;
            }
        }

        public string sName
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        public string sValue
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }

        public string sType
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        }

        public string sLabel
        {
            get
            {
                return Label;
            }
            set
            {
                Label = value;
            }
        }

        public decimal fOrder
        {
            get
            {
                return Order;
            }
            set
            {
                Order = value;
            }
        }

        public string sContext
        {
            get
            {
                return Context;
            }
            set
            {
                Context = value;
            }
        }

        public string sPreviousValue { get; set; }
        public CNV NInstance(string sKey)
        {
            CNV oInstance;

            if (NNVs.ContainsKey(sKey))
            {
                oInstance = NNVs[sKey];
            }
            else
            {
                oInstance = new CNV();
                NNVs.Add(sKey, oInstance);
                oInstance.sName = sKey;
            }

            return oInstance;
        }
    }
}