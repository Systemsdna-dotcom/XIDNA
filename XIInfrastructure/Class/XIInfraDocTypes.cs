using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using XISystem;
using XIDatabase;
using System.Configuration;

namespace XIInfrastructure
{
    [Table("XIDocTypes")]
    public class XIInfraDocTypes
    {
        [Key]
        public int ID { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string SubDirectory { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult Get_FileDocTypes(string Type="", string sID = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            try
            {
                XIInfraDocTypes oXIDocTypes = null;
                //cConnectionString oConString = new cConnectionString();
                //string sConString = oConString.ConnectionString(sCoreDataBase);
                //XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                int iID = 0;
                if (!string.IsNullOrEmpty(sID))
                {
                    if (int.TryParse(sID, out iID))
                    {
                        Params["ID"] = iID;
                    }
                }
                if (ID!=0)
                {
                    Params["ID"] = ID;
                }
                if(!string.IsNullOrEmpty(Type))
                {
                    Params["Type"] = Type;
                }
                oXIDocTypes = Connection.Select<XIInfraDocTypes>("XIDocTypes", Params).FirstOrDefault();
                oCResult.oResult = oXIDocTypes;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
    }
}