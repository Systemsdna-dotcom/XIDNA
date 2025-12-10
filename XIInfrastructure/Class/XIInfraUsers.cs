using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XISystem;
using XIDatabase;
using XICore;
using static XIDatabase.XIDBAPI;

namespace XIInfrastructure
{
    [Table("XIAPPUsers_AU_T")]
    public class XIInfraUsers : XIDefinitionBase
    {
        [Key]
        public int UserID { get; set; }
        public int FKiApplicationID { get; set; }
        public int FKiOrganisationID { get; set; }
        public string sUserName { get; set; }
        public string sPasswordHash { get; set; }
        public string sDatabaseName { get; set; }
        public string sCoreDatabaseName { get; set; }
        public string sAppName { get; set; }
        public string sLocation { get; set; }
        public string sPhoneNumber { get; set; }
        public string sEmail { get; set; }
        public string sFirstName { get; set; }
        public string sLastName { get; set; }
        public string sCol0 { get; set; }
        public string sRow1 { get; set; }
        public string sRow2 { get; set; }
        public string sRow3 { get; set; }
        public string sRow4 { get; set; }
        public string sRow5 { get; set; }
        public int iReportTo { get; set; }
        public int iPaginationCount { get; set; }
        public string sMenu { get; set; }
        public int iInboxRefreshTime { get; set; }
        public string SecurityStamp { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string sTemporaryPasswordHash { get; set; }
        [DapperIgnore]
        public bool bIsActive { get; set; }

        [NotMapped]
        private XIInfraUserRoles oMyRoleID;
        [NotMapped]
        public XIInfraUserRoles RoleID
        {
            get
            {
                return oMyRoleID;
            }
            set
            {
                oMyRoleID = value;
            }
        }

        [NotMapped]
        private XIInfraRoles oMyRole;
        [NotMapped]
        public XIInfraRoles Role
        {
            get
            {
                return oMyRole;
            }
            set
            {
                oMyRole = value;
            }
        }

        public CResult Get_UserDetails(string sCoreDatabase)
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
                XIInfraUsers xifUser = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(sEmail))
                {
                    Params["sEmail"] = sEmail;
                }
                else if (UserID > 0)
                {
                    Params["UserID"] = UserID;
                }
                else if (!string.IsNullOrEmpty(sUserName))
                {
                    Params["sUserName"] = sUserName;
                }
                xifUser = Connection.Select<XIInfraUsers>("XIAPPUsers_AU_T", Params).FirstOrDefault();
                if (xifUser != null)
                {
                    XIInfraUserRoles xifURole = new XIInfraUserRoles();
                    xifURole.UserID = xifUser.UserID;
                    var oURoleData = xifURole.Get_UserRoles(sCoreDatabase);
                    if (oURoleData.bOK)
                    {
                        xifURole = ((List<XIInfraUserRoles>)oURoleData.oResult).FirstOrDefault();
                        xifUser.RoleID = xifURole;
                        if (xifURole.RoleID > 0)
                        {
                            XIInfraRoles xifRole = new XIInfraRoles();
                            xifRole.RoleID = xifURole.RoleID;
                            var oRoleData = xifRole.Get_RoleDefinition(sCoreDatabase);
                            if (oRoleData.bOK)
                            {
                                xifRole = (XIInfraRoles)oRoleData.oResult;
                                xifUser.Role = xifRole;
                            }
                        }
                        if (xifUser.Role.sRoleName.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower())
                        {
                            cConnectionString oSharedConString = new cConnectionString();
                            string sSharedConString = oSharedConString.ConnectionString(xifUser.sDatabaseName);
                            XIDBAPI SharedConnection = new XIDBAPI(sSharedConString);
                            Dictionary<string, object> CustParams = new Dictionary<string, object>();
                            CustParams["fkiuserid"] = xifUser.UserID;
                            var Customer = SharedConnection.SelectString("iStatus", "customer_t", CustParams);//.Select<XIInfraUsers>("XIAPPUsers_AU_T", Params).FirstOrDefault();
                            if (Customer == null)
                            {
                                xifUser.bIsActive = true;
                            }
                            else
                            {
                                int Custact = 1;
                                if (int.TryParse(Customer, out Custact))
                                {
                                    xifUser.bIsActive = Custact != 0 ? false : true;
                                }
                            }
                        }
                        else
                        {
                            xifUser.bIsActive = true;
                        }                        
                    }
                }
                oCResult.oResult = xifUser;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }
        public CResult Save_User(string sCoreDatabase)
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
                XIInfraUsers User = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                User = Connection.Insert<XIInfraUsers>(this, "XIAPPUsers_AU_T", "UserID");
                oCResult.oResult = User;
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

        public CResult Update_User(string sCoreDatabase)
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
                XIInfraUsers User = null;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                User = Connection.Update<XIInfraUsers>(this, "XIAPPUsers_AU_T", "UserID");
                oCResult.oResult = User;
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

        public CResult Get_NoOfUsers(string sCoreDatabase)
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
                List<XIInfraUsers> xifUser = null;
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["FKiOrganisationID"] = FKiOrganisationID;
                cConnectionString oConString = new cConnectionString();
                string sConString = oConString.ConnectionString(sCoreDatabase);
                XIDBAPI Connection = new XIDBAPI(sConString);
                xifUser = Connection.Select<XIInfraUsers>("XIAPPUsers_AU_T", Params).ToList();
                oCResult.oResult = xifUser.Count();
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always

            //return oURL;
        }

        //public XIInfraUsers UserDetails(string sCoreDatabase)
        //{
        //    XIInfraDbContext dbContext = new XIInfraDbContext(sCoreDatabase);
        //    XIInfraUsers oUser = null;
        //    if (!string.IsNullOrEmpty(sUserName))
        //    {
        //        oUser = dbContext.XIAppUsers.Where(m => m.sUserName == sUserName).FirstOrDefault();
        //    }
        //    else if (UserID > 0)
        //    {
        //        oUser = dbContext.XIAppUsers.Where(m => m.UserID == UserID).FirstOrDefault();
        //    }
        //    if (oUser != null)
        //    {
        //        oUser.RoleID = dbContext.XIAppUserRoles.Where(m => m.UserID == oUser.UserID).FirstOrDefault();
        //        if (oUser.RoleID != null)
        //        {
        //            oUser.Role = dbContext.XIAppRoles.Where(m => m.RoleID == oUser.RoleID.RoleID).FirstOrDefault();
        //        }
        //    }
        //    return oUser;
        //}

    }

}