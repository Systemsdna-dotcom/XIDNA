using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class cXICache
    {
        public string Get_ParamVal(string sSessionID, string sUID, string sContext, string sParamName)
        {
            CInstance oCache = Get_XICache();
            string sRuntimeVal = string.Empty;
            if (sUID != null)
            {
                if (!string.IsNullOrEmpty(sContext))
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sValue;
                }
                else
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue;
                }

            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return sRuntimeVal;
        }

        public CInstance Get_Paramobject(string sSessionID, string sUID, string sContext, string sParamName)
        {
            CInstance oCache = Get_XICache();
            CInstance sRuntimeVal = new CInstance();
            if (sUID != null)
            {
                if (!string.IsNullOrEmpty(sContext))
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName);
                }
                else
                {
                    sRuntimeVal = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName);
                }

            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return sRuntimeVal;
        }

        public string Set_ParamVal(string sSessionID, string sUID, string sContext, string sParamName, string sParamValue, string sType, List<cNameValuePairs> nSubParams)
        {
            CInstance oCache = Get_XICache();
            if (sUID != null && sUID.Length > 0)
            {
                if (!string.IsNullOrEmpty(sContext))
                {
                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sValue = sParamValue;
                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext).NInstance(sParamName).sType = sType;
                }
                else
                {
                    if (sType != null && sType.ToLower() == "register".ToLower())
                    {
                        if (oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers == null)
                        {
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers = new List<cNameValuePairs>();
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new cNameValuePairs { sName = sParamName, sValue = sParamValue });
                        }
                        else
                        {
                            var IsExists = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Where(m => m.sValue == sParamValue).FirstOrDefault();
                            if (IsExists == null)
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new cNameValuePairs { sName = sParamName, sValue = sParamValue });
                            }
                        }
                    }
                    else
                    {
                        if (oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue != null)
                        {
                            if (!sParamValue.StartsWith("{XIP|"))
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sType = sType;
                                if (nSubParams != null && nSubParams.Count() > 0)
                                {
                                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                                }
                            }
                            else
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                                if(nSubParams !=null && nSubParams.Count() > 0)
                                {
                                    oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                                }
                            }
                        }
                        else
                        {
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sType = sType;
                            if (nSubParams != null && nSubParams.Count() > 0)
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams = nSubParams;
                            }
                        }

                    }
                }
            }
            return "TRUE";
        }

        public CInstance GetAllParamsUnderGUID(string sSessionID, string sUID, string sContext)
        {
            CInstance oCache = Get_XICache();
            CInstance oGUIDParams = new CInstance();
            if (!string.IsNullOrEmpty(sContext))
            {
                oGUIDParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("Con_" + sContext);
            }
            else
            {
                oGUIDParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID);
            }

            return oGUIDParams;
        }

        public CInstance Get_XICache()
        {
            object obj;
            CInstance oCacheobj = new CInstance();
            if (HttpRuntime.Cache["XICache"] == null)
            {
                CInstance oCache = new CInstance();
                HttpRuntime.Cache.Add("XICache", oCache, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                //oCache = HttpRuntime.Cache["XICache"];
            }
            else
            {
                obj = HttpRuntime.Cache["XICache"];
                oCacheobj = (CInstance)obj;
                return oCacheobj;
            }
            Get_XICache();
            return oCacheobj;
        }

        public CInstance Init_RuntimeParamSet(string sSessionID, string sNewUID, string sParentUID = "", string sContext = "")
        {
            CInstance oCache = Get_XICache();
            CInstance oNewRTInst;
            if (!string.IsNullOrEmpty(sContext))
            {
                oNewRTInst = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sNewUID).NInstance("Con_" + sContext);
            }
            else
            {
                oNewRTInst = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sNewUID);
            }

            if (sParentUID != "") { oNewRTInst.NInstance("|XIParent").sValue = sParentUID; }
            return oNewRTInst;
        }

        public string Set_ActiveInstance(string sSessionID, string LayoutID, string sParamName, string sParamValue)
        {
            CInstance oCache = Get_XICache();
            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("LID_" + LayoutID).NInstance(sParamName).sValue = sParamValue;
            return "TRUE";
        }

        public string ReplaceExpressionWithCacheValue(string NewQuery, List<cNameValuePairs> nParams)
        {
            var sSessionID = HttpContext.Current.Session.SessionID;
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(NewQuery);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var Prm = "{" + match.ToString() + "}";
                    var Matches = match.ToString();
                    var sExpr = match.ToString().Replace("{", "").Replace("}", "");
                    var Value = nParams.Where(m => m.sName.ToString().ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Value))
                    {
                        NewQuery = NewQuery.Replace("{" + match.ToString() + "}", "'"+Value+"'");
                    }
                }
            }
            return NewQuery;
        }

        public void ClearCache()
        {
            IDictionaryEnumerator cacheEnumerator = HttpContext.Current.Cache.GetEnumerator();
            while (cacheEnumerator.MoveNext())
            {
                HttpContext.Current.Cache.Remove(cacheEnumerator.Key.ToString());
            }
        }

        #region CacheObject
        public object GetObjectFromCache(string ObjName, string sGUID, int iUserID, string sOrgName, string sDatabase, int ID = 0, string sCurrentGuestUser = "")
        {
            //ClearCache();
            var UserDetais = GetUserDetails(iUserID, sOrgName, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            var CacheStatus = ServiceUtil.GetCacheStatus();
            object oXiLink = new object();
            if (CacheStatus != "OFF")
            {
                string Key = string.Empty;
                if (!string.IsNullOrEmpty(UserDetais.sAppName))
                {
                    Key = UserDetais.sAppName + "_" + UserDetais.FKiOrgID + "_" + ObjName + "_" + ID;
                }
                else
                {
                    Key = UserDetais.FKiOrgID + "_" + ObjName + "_" + ID;
                }

                if (HttpRuntime.Cache[Key] == null)
                {
                    var Obj = AddObjectToCache(ObjName, iUserID, sOrgName, sDatabase, ID, sCurrentGuestUser);
                    HttpRuntime.Cache.Insert(Key, Obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    oXiLink = HttpRuntime.Cache[Key];
                }
                else
                {
                    oXiLink = HttpRuntime.Cache[Key];
                }
                //return ((IXiLinkRepository)oXiLink).DeepClone();
            }
            else
            {
                var Obj = AddObjectToCache(ObjName, iUserID, sOrgName, sDatabase, ID, sCurrentGuestUser);
                oXiLink = Obj;
            }
            return oXiLink;
        }

        private object AddObjectToCache(string ObjName, int iUserID, string sOrgName, string sDatabase, int ID = 0, string sCurrentGuestUser = "")
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinkRepository oQSRepo = new XiLinkRepository();
            switch (ObjName)
            {
                case "XiLink":
                    var XiLinks = dbContext.XiLinks.Find(ID);
                    return XiLinks;
                case "QuestionSet":
                    var oQSDef = oQSRepo.GetQuestionSetDefinitionByID(ID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                    return oQSDef;
                case "Step":
                    var oStep = oQSRepo.GetStepDefinitionByID(ID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                    return oStep;
                case "Layout":
                    var oLayout = oQSRepo.GetLayoutByID(ID);
                    return oLayout;
                default:
                    return null;
            }
        }

        public void InsertIntoCache(object oCacheObj, string sCacheKey)
        {
            HttpRuntime.Cache.Insert(sCacheKey, oCacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public object GetFromCache(string sCacheKey)
        {
            object oCacheObj = HttpRuntime.Cache[sCacheKey];
            return oCacheObj;
        }

        public void UpdateCacheObject(string ObjName, string sGUID, object Obj, string sDatabase, int ID = 0)
        {
            HttpRuntime.Cache.Insert(ObjName + "_" + sGUID + "_" + ID, Obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            var QSet = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
        }

        public XiLinks GetXiLinkDetails(int XiLinkID, string sGUID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinks oXilink = new XiLinks();
            oXilink = (XiLinks)GetObjectFromCache("XILink", sGUID, iUserID, sOrgName, sDatabase, XiLinkID);
            return oXilink;
        }

        private VM_cXIAppUserDetails GetUserDetails(int UserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VM_cXIAppUserDetails oUser = new VM_cXIAppUserDetails();
            if (UserID > 0)
            {
                var User = dbCore.XIAppUsers.Find(UserID);
                var RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
                var Role = dbCore.XIAppRoles.Find(RoleID);
                oUser.UserID = UserID;
                if (User != null)
                {
                    oUser.sCoreDatabase = User.sCoreDatabaseName;
                    oUser.sUserDatabase = User.sDatabaseName;
                    oUser.sRoleName = Role.sRoleName;
                    oUser.iRoleID = RoleID;
                    oUser.FKiOrgID = User.FKiOrganisationID;
                    oUser.iThemeID = Role.iThemeID;
                    oUser.iLayoutID = Role.iLayoutID;
                    oUser.sUserName = User.sUserName;
                    oUser.sAppName = User.sAppName;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(sOrgName))
                {
                    var OrgDetails = dbCore.Organization.Where(m => m.Name == sOrgName).FirstOrDefault();
                    oUser.FKiOrgID = OrgDetails.ID;
                    oUser.sUserDatabase = OrgDetails.DatabaseName;
                }
                return oUser;
            }
            return oUser;
        }

        #endregion CacheObject

    }
}
