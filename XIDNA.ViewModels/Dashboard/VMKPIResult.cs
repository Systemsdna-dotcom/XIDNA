using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMKPIResult
    {
        public string Name { get; set; }
        public string KPIValue { get; set; }
        public int KPIPercent { get; set; }
        public string KPICircleColor { get; set; }
        public string KPIIconColor { get; set; }
        public string KPIIcon { get; set; }
        public List<DashBoardGraphs> AllLeadsCount { get; set; }
        public string SectionName { get; set; }
        public int? TabID { get; set; }
        public int ReportID { get; set; }
        public int InnerReportID { get; set; }
        public string Visibility { get; set; }
        //public string GlobalVisibility { get; set; }
        public int UserID { get; set; }
        public string ShowAs { get; set; }
    }

    public class KPICircleColors : List<Color>
    {
        public KPICircleColors()
        {
            this.Add(new Color() { KPIColor = "easypie-darksalmon" });
            this.Add(new Color() { KPIColor = "easypie-palevioletred" });
            this.Add(new Color() { KPIColor = "easypie-goldenrod" });
            this.Add(new Color() { KPIColor = "easypie-lightcora" });
            this.Add(new Color() { KPIColor = "easypie-darkslateblue" });
            this.Add(new Color() { KPIColor = "easypie-skyblue" });
            this.Add(new Color() { KPIColor = "easypie-yellowgreen" });
            this.Add(new Color() { KPIColor = "easypie-easypie-lightslategray" });
            this.Add(new Color() { KPIColor = "easypie-coral" });
            this.Add(new Color() { KPIColor = "easypie-mediumpurple" });
            this.Add(new Color() { KPIColor = "easypie-lightseagreen" });
            this.Add(new Color() { KPIColor = "easypie-steelblue" });
            this.Add(new Color() { KPIColor = "easypie-turquoise" });
            this.Add(new Color() { KPIColor = "easypie-darksalmon" });
            this.Add(new Color() { KPIColor = "easypie-turquoise" });
            this.Add(new Color() { KPIColor = "easypie-darkgray" });
            this.Add(new Color() { KPIColor = "easypie-gold" });
            this.Add(new Color() { KPIColor = "easypie-mediumturquoise" });
            this.Add(new Color() { KPIColor = "easypie-dimgray" });
        }
    }

    public class KPIIconColors : List<Color>
    {
        public KPIIconColors()
        {
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
            this.Add(new Color() { KPIColor = "chart-topicon" });
        }
    }

    public class KPIIcon : List<Color>
    {
        public KPIIcon()
        {
            this.Add(new Color() { KPIColor = "fa fa-user" });
            this.Add(new Color() { KPIColor = "fa fa-tags" });
            this.Add(new Color() { KPIColor = "fa fa-bar-chart-o" });
            this.Add(new Color() { KPIColor = "fa fa-comments" });
            this.Add(new Color() { KPIColor = "fa fa-eye" });
            this.Add(new Color() { KPIColor = "fa fa-link" });
            this.Add(new Color() { KPIColor = "fa fa-user" });
        }
    }

    public class Color
   {
       public string KPIColor { get; set; }
   }
}
