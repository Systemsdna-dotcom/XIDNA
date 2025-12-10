using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace XISystem
{
    public class CUserInfo
    {
        public int iUserID { get; set; }
        public string sUserName { get; set; }
        public string sEmail { get; set; }
        public string sRoleName { get; set; }
        public int iRoleID { get; set; }
        public bool bDBAccess { get; set; }

        public int iOrganizationID { get; set; }
        public string iProjectID { get; set; }
        public string sOrgName { get; set; }
        public string sName { get; set; }
        public string AppName { get; set; }
        public string sCoreDataBase { get; set; }
        public string sDatabaseName { get; set; }
        public string sHierarchy { get; set; }
        public string sInsertDefaultCode { get; set; }
        public string sUpdateHierarchy { get; set; }
        public string sViewHierarchy { get; set; }
        public string sDeleteHierarchy { get; set; }
        public int iApplicationID { get; set; }
        public Guid iApplicationIDXIGUID { get; set; }
        public int iActorID { get; set; }
        public string sActorName { get; set; }
        public string sWhereField { get; set; }
        public int iWhereFieldValue { get; set; }
        public int iLevel { get; set; }
        public int FKiTeamID { get;set; }
        private CUserInfo GetInfoFromObject(List<CNV> oData)
        {
            CUserInfo oUInfo = new CUserInfo();
            if (oData != null)
            {
                oUInfo.sUserName = oData.Where(m => m.sName.ToLower() == "sUserName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oUInfo.sRoleName = oData.Where(m => m.sName.ToLower() == "sRoleName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oUInfo.sEmail = oData.Where(m => m.sName.ToLower() == "sEmail".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oUInfo.sName = oData.Where(m => m.sName.ToLower() == "sName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var UserID = oData.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oUInfo.AppName = oData.Where(m => m.sName.ToLower() == "AppName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (UserID != null)
                {
                    int iUID = 0;
                    if (int.TryParse(UserID, out iUID))
                    {
                        oUInfo.iUserID = iUID;
                    }
                }
                var OrgID = oData.Where(m => m.sName.ToLower() == "iOrganizationID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (OrgID != null)
                {
                    int iOrgID = 0;
                    if (int.TryParse(OrgID, out iOrgID))
                    {
                        oUInfo.iOrganizationID = iOrgID;
                    }
                }
                oUInfo.sOrgName = oData.Where(m => m.sName.ToLower() == "sOrgName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oUInfo.sCoreDataBase = oData.Where(m => m.sName.ToLower() == "sDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var RoleID = oData.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (RoleID != null)
                {
                    int iRoleID = 0;
                    if (int.TryParse(RoleID, out iRoleID))
                    {
                        oUInfo.iRoleID = iRoleID;
                    }
                }
            }
            return oUInfo;
        }

        public CUserInfo GetUserInfo(List<CNV> SessionItems = null)
        {
            if (SessionItems != null && SessionItems.Count > 0)
            {
                CUserInfo oInfo = new CUserInfo();
                oInfo = GetInfoFromObject(SessionItems);
                return oInfo;
            }
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XIDNA.dll";
            //var DLL = Assembly.LoadFile(sPath + "\\XIInfrastructure.dll");
            string StrPath = sPath; //"D:\\TfsProjects\\XIDNAOM\\XIDNA\\XIInfrastructure\\bin\\XIInfrastructure.dll";
            //Creating a new appdomain
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain
            //System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(LoadMyAssembly).Assembly.FullName, typeof(LoadMyAssembly).FullName);
            //LoadMyAssembly loader = (LoadMyAssembly)obj.Unwrap();//As the object we are creating is from another appdomain hence we will get that object in wrapped format and hence in next step we have unwrappped it
            //Call loadassembly method so that the assembly will be loaded into the new appdomain amd the object will also remain in new appdomain only.

            //loader.LoadAssembly(StrPath);
            //Call exceuteMethod and pass the name of the method from assembly and the parameters.
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (String.Compare(type.Name, "SessionManager", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }
            }
            MethodInfo MyMethod = Ltype.GetMethod("SessionItems");
            object[] parametersArray = new object[] { };
            object Response = (object)MyMethod.Invoke(objclass, parametersArray);

            //object Response = loader.ExecuteStaticMethod("GetObjectFromCache", new object[] { sSessionID, ObjType, ObjName, ObjID });
            AppDomain.Unload(newDomain); //After the method has been executed call unload method of the appdomain.
            CUserInfo oUInfo = new CUserInfo();
            if (Response != null)
            {
                var oData = (List<CNV>)Response;
                oUInfo = GetInfoFromObject(oData);
                //oUInfo.sUserName = oData.Where(m => m.sName.ToLower() == "sUserName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //oUInfo.sRoleName = oData.Where(m => m.sName.ToLower() == "sRoleName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //oUInfo.sEmail = oData.Where(m => m.sName.ToLower() == "sEmail".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //oUInfo.sName = oData.Where(m => m.sName.ToLower() == "sName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //var UserID = oData.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //oUInfo.AppName = oData.Where(m => m.sName.ToLower() == "AppName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //if (UserID != null)
                //{
                //    int iUID = 0;
                //    if (int.TryParse(UserID, out iUID))
                //    {
                //        oUInfo.iUserID = iUID;
                //    }
                //}
                //var OrgID = oData.Where(m => m.sName.ToLower() == "iOrganizationID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //if (OrgID != null)
                //{
                //    int iOrgID = 0;
                //    if (int.TryParse(OrgID, out iOrgID))
                //    {
                //        oUInfo.iOrganizationID = iOrgID;
                //    }
                //}
                //oUInfo.sOrgName = oData.Where(m => m.sName.ToLower() == "sOrgName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //oUInfo.sCoreDataBase = oData.Where(m => m.sName.ToLower() == "sDatabase".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //var RoleID = oData.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //if (RoleID != null)
                //{
                //    int iRoleID = 0;
                //    if (int.TryParse(RoleID, out iRoleID))
                //    {
                //        oUInfo.iRoleID = iRoleID;
                //    }
                //}
            }
            return oUInfo;
            //Wow you have unloaded the new appdomain and also unloaded the loaded assembly from memory.
        }
    }
}