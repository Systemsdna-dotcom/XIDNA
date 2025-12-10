using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDatabase;
using XISystem;
using XICore;

namespace XIInfrastructure
{
    [Table("XIAppRoles_AR_T")]
    public class XIInfraRoles
    {
        public XIInfraRoles()
        {
            this.SubGroups = new HashSet<XIInfraRoles>();
        }

        [Key]
        public int RoleID { get; set; }
        public int iParentID { get; set; }
        public string sRoleName { get; set; }
        public int FKiOrganizationID { get; set; }
        public int iLayoutID { get; set; }
        public int iThemeID { get; set; }
        [XIDBAPI.DapperIgnore]
        public virtual ICollection<XIInfraRoles> SubGroups { get; set; }
        public virtual XIInfraRoles groups { get; set; }
        public bool IsLeaf
        {
            get
            {
                return this.SubGroups.Count == 0;
            }
        }
        [XIDBAPI.DapperIgnore]
        [NotMapped]
        public int ReportToID { get; set; }
        [XIDBAPI.DapperIgnore]
        [NotMapped]
        public int UserID { get; set; }
        [NotMapped]
        public List<XIDropDown> LayoutsList { get; set; }
        [NotMapped]
        public List<XIDropDown> ThemesList { get; set; }

        public CResult Get_RoleDefinition(string sCoreDatabase)
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
                XIInfraRoles oRole = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (RoleID > 0)
                {
                    Params["RoleID"] = RoleID;
                }
                if (!string.IsNullOrEmpty(sRoleName))
                {
                    Params["sRoleName"] = sRoleName;
                }
                oRole = Connection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).FirstOrDefault();
                oCResult.oResult = oRole;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult Get_RolesTree(string sCoreDatabase, int iOrgID)
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
                List<XIInfraRoles> oRole = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["iParentID"] = 2;
                Params["FKiOrganizationID"] = iOrgID;
                oRole = Connection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).ToList();
                var RoleTree = GetTree(oRole, sCoreDatabase);
                oCResult.oResult = RoleTree;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public List<XIInfraRoles> GetTree(List<XIInfraRoles> oTopRole, string sCoreDatabase)
        {
            foreach (var items in oTopRole)
            {
                var ID = items.RoleID;
                List<XIInfraRoles> oSubRoles = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["iParentID"] = ID;
                oSubRoles = Connection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).ToList();
                items.SubGroups = oSubRoles.ToList();
                if (items.SubGroups.Count() > 0)
                {
                    GetTree(items.SubGroups.ToList(), sCoreDatabase);
                }
            }
            return oTopRole;
        }

        public CResult Save_Role(string sCoreDatabase)
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

            try
            {
                XIInfraRoles xifRole = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                if (this.RoleID == 0)
                {
                    xifRole = Connection.Insert<XIInfraRoles>(this, "XIAppRoles_AR_T", "RoleID");
                }
                else
                {
                    xifRole = Connection.Update<XIInfraRoles>(this, "XIAppRoles_AR_T", "RoleID");
                }
                oCResult.oResult = xifRole;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public string GetRoleTheme(string sCoreDatabase)
        {
            string sThemeFile = string.Empty;
            cConnectionString oConString = new cConnectionString();
            string sConString = oConString.ConnectionString(sCoreDatabase);
            XIDBAPI Connection = new XIDBAPI(sConString);
            Dictionary<string, object> UserParams = new Dictionary<string, object>();
            UserParams["ID"] = iThemeID;
            sThemeFile = Connection.SelectString("FileName", "XIMasterData_T", UserParams).ToString();
            return sThemeFile;
        }

        public CResult Get_RoleByID(int RoleID = 0, string sCoreDatabase = "")
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

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

            try
            {
                XIInfraRoles oRole = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["RoleID"] = RoleID;
                oRole = Connection.Select<XIInfraRoles>("XIAppRoles_AR_T", Params).FirstOrDefault();
                oCResult.oResult = oRole;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
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