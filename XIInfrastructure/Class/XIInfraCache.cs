using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraCache
    {
        public string sSessionID { get; set; }
        public string sParGUID = string.Empty;
        public string sKey { get; set; }
        public string oCachedObject { get; set; }
        public string sSize { get; set; }
        public void SetXIParams(List<CNV> oParams, string sGUID, string sSessionID)
        {
            XIInfraCache oCache = new XIInfraCache();
            // string sSessionID = HttpContext.Current.Session.SessionID;
            foreach (var items in oParams)
            {
                if (!string.IsNullOrEmpty(items.sValue))
                {
                    oCache.Set_ParamVal(sSessionID, sGUID, items.sContext, items.sName, items.sValue, items.sType, items.nSubParams);
                }
            }
        }

        public string Get_ParamVal(string sSessionID, string sUID, string sContext, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            string sRuntimeVal = string.Empty;
            string sParamContext = string.Empty;
            if (sUID != null)
            {
                if (sParamName.StartsWith("-") && sParamName.Contains("."))
                {
                    var Splithiphen = sParamName.Split('.').ToList();
                    sParamContext = Splithiphen[0];
                    sParamName = Splithiphen[1];
                    if (!string.IsNullOrEmpty(sParamContext))
                    {
                        var subParams = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).nSubParams;
                        if (subParams != null)
                        {
                            sRuntimeVal = subParams.Where(m => m.sName.ToLower() == sParamName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        }
                    }
                }
                else
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
            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return sRuntimeVal;
        }

        public XICacheInstance Get_Paramobject(string sSessionID, string sUID, string sContext, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance sRuntimeVal = new XICacheInstance();
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

        public string Set_ParamVal(string sSessionID, string sUID, string sContext, string sParamName, string sParamValue, string sType, List<CNV> nSubParams)
        {
            XICacheInstance oCache = Get_XICache();
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
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers = new List<CNV>();
                            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new CNV { sName = sParamName, sValue = sParamValue });
                        }
                        else
                        {
                            var IsExists = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Where(m => m.sValue == sParamValue).FirstOrDefault();
                            if (IsExists == null)
                            {
                                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).Registers.Add(new CNV { sName = sParamName, sValue = sParamValue });
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
                                if (nSubParams != null && nSubParams.Count() > 0)
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
        public string Set_ScriptVal(string sSessionID, string sParamName, MethodInfo sParamValue)
        {
            XICacheInstance oCache = Get_XICache();
            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance(sParamName).Script = sParamValue;
            return "TRUE";
        }
        public MethodInfo Get_ScriptVal(string sSessionID, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            MethodInfo oMethodInfo = null;
            oMethodInfo = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance(sParamName).Script;
            return oMethodInfo;
        }

        public string Set_QsStructureObj(string sSessionID, string sUID, string sParamName, XIBOInstance sParamValue)
        {
            XICacheInstance oCache = Get_XICache();
            if (!string.IsNullOrEmpty(sUID))
            {
                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).oBOInstance = sParamValue;
            }
            return "TRUE";
        }
        public XIBOInstance Get_QsStructureObj(string sSessionID, string sUID, string sParamName)
        {
            XICacheInstance oCache = Get_XICache();
            XIBOInstance oBOInstance = new XIBOInstance();
            if (!string.IsNullOrEmpty(sUID))
            {
                oBOInstance = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).oBOInstance;
            }
            return oBOInstance;
        }

        public XICacheInstance GetAllParamsUnderGUID(string sSessionID, string sUID, string sContext)
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance oGUIDParams = new XICacheInstance();
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

        public XICacheInstance Get_XICache()
        {
            object obj;
            XICacheInstance oCacheobj = new XICacheInstance();
            if (HttpRuntime.Cache["XICache"] == null)
            {
                XICacheInstance oCache = new XICacheInstance();
                HttpRuntime.Cache.Add("XICache", oCache, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                //oCache = HttpRuntime.Cache["XICache"];
            }
            else
            {
                obj = HttpRuntime.Cache["XICache"];
                oCacheobj = (XICacheInstance)obj;
                return oCacheobj;
            }
            Get_XICache();
            return oCacheobj;
        }

        public XICacheInstance Init_RuntimeParamSet(string sSessionID, string sNewUID, string sParentUID = "", string sContext = "")
        {
            XICacheInstance oCache = Get_XICache();
            XICacheInstance oNewRTInst;
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
            XICacheInstance oCache = Get_XICache();
            oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("LID_" + LayoutID).NInstance(sParamName).sValue = sParamValue;
            return "TRUE";
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

        //Get Object from Cache if not exists insert and get it
        public object GetObjectFromCache(string ObjType = "", string ObjName = "", string ObjID = "")
        {
            //var sSessionID = HttpContext.Current.Session.SessionID;
            var CacheKey = CacheKeyBuilder(ObjType, ObjName, ObjID);
            var CacheStatus = ConfigurationManager.AppSettings["Cache"];
            object oCacheObj;
            if (CacheStatus != "OFF")
            {
                if (HttpRuntime.Cache[CacheKey] == null)
                {
                    var CacheObj = AddObjectToCache(ObjType, ObjName, ObjID);
                    HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                    if (ObjType == "bo")
                    {
                        var BOD = (XIDBO)CacheObj;
                        if (BOD != null)
                        {
                            if (!string.IsNullOrEmpty(ObjName))
                            {
                                CacheKey = CacheKeyBuilder(ObjType, null, BOD.BOID.ToString());
                                HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                            }
                            else if (!string.IsNullOrEmpty(ObjID) && ObjID != "0")
                            {
                                CacheKey = CacheKeyBuilder(ObjType, BOD.Name, null);
                                HttpRuntime.Cache.Insert(CacheKey, CacheObj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                            }
                        }
                    }
                    oCacheObj = HttpRuntime.Cache[CacheKey];
                }
                else
                {
                    oCacheObj = HttpRuntime.Cache[CacheKey];
                }
            }
            else
            {
                oCacheObj = AddObjectToCache(ObjType, ObjName, ObjID);
            }
            return oCacheObj;
        }

        private object AddObjectToCache(string ObjType, string ObjName, string ObjID = "")
        {
            XIDXI oDXI = new XIDXI();
            switch (ObjType.ToLower())
            {
                case "bo":
                    XIDBO xiBOD = new XIDBO();
                    var oBODef = oDXI.Get_BODefinition(ObjName, ObjID);
                    if (oBODef.bOK && oBODef.oResult != null)
                    {
                        xiBOD = (XIDBO)oBODef.oResult;
                    }
                    return xiBOD;
                case "bo_all":
                    XIDBO xiBODAll = new XIDBO();
                    var xiBODef = oDXI.Get_BODefinitionALL(ObjName, ObjID);
                    if (xiBODef.bOK && xiBODef.oResult != null)
                    {
                        xiBODAll = (XIDBO)xiBODef.oResult;
                    }
                    return xiBODAll;
                case "xilink":
                    XILink xiLink = new XILink();
                    var oXiLinkDef = oDXI.Get_XILinkDefinition(ObjID, ObjName);
                    if (oXiLinkDef.bOK && oXiLinkDef.oResult != null)
                    {
                        xiLink = (XILink)oXiLinkDef.oResult;
                    }
                    return xiLink;
                case "url":
                    XIURLMappings xiURL = new XIURLMappings();
                    var oXiURLDef = oDXI.Get_URLDefinition(ObjName);
                    if (oXiURLDef.bOK && oXiURLDef.oResult != null)
                    {
                        xiURL = (XIURLMappings)oXiURLDef.oResult;
                    }
                    return xiURL;
                case "application":
                    XIDApplication xiApp = new XIDApplication();
                    var oXiAppDef = oDXI.Get_ApplicationDefinition(ObjName);
                    if (oXiAppDef.bOK && oXiAppDef.oResult != null)
                    {
                        xiApp = (XIDApplication)oXiAppDef.oResult;
                    }
                    return xiApp;
                case "organisation":
                    XIDOrganisation xiOrg = new XIDOrganisation();
                    var oXiOrgDef = oDXI.Get_OrgDefinition(ObjName, ObjID);
                    if (oXiOrgDef.bOK && oXiOrgDef.oResult != null)
                    {
                        xiOrg = (XIDOrganisation)oXiOrgDef.oResult;
                    }
                    return xiOrg;
                case "layout":
                    XIDLayout xiLayout = new XIDLayout();
                    var oXiLayoutDef = oDXI.Get_LayoutDefinition(ObjName, ObjID);
                    if (oXiLayoutDef.bOK && oXiLayoutDef.oResult != null)
                    {
                        xiLayout = (XIDLayout)oXiLayoutDef.oResult;
                    }
                    return xiLayout;
                case "questionset":
                    XIDQS xiQS = new XIDQS();
                    var oQSDef = oDXI.Get_QSDefinition(ObjName, ObjID);
                    if (oQSDef.bOK && oQSDef.oResult != null)
                    {
                        xiQS = (XIDQS)oQSDef.oResult;
                    }
                    return xiQS;
                case "qsstep":
                    XIDQSStep xiQSStep = new XIDQSStep();
                    var oQSStepDef = oDXI.Get_StepDefinition(ObjName, ObjID);
                    if (oQSStepDef.bOK && oQSStepDef.oResult != null)
                    {
                        xiQSStep = (XIDQSStep)oQSStepDef.oResult;
                    }
                    return xiQSStep;
                case "oneclick":
                    XID1Click o1ClickD = new XID1Click();
                    var o1ClickDef = oDXI.Get_1ClickDefinition(ObjName, ObjID);
                    if (o1ClickDef.bOK && o1ClickDef.oResult != null)
                    {
                        o1ClickD = (XID1Click)o1ClickDef.oResult;
                    }
                    return o1ClickD;
                case "component":
                    XIDComponent oComponentD = new XIDComponent();
                    var oCompnDef = oDXI.Get_ComponentDefinition(ObjName, ObjID);
                    if (oCompnDef.bOK && oCompnDef.oResult != null)
                    {
                        oComponentD = (XIDComponent)oCompnDef.oResult;
                    }
                    return oComponentD;
                case "datasource":
                    XIDataSource oDataSourceD = new XIDataSource();
                    var oDataSourceDedf = oDXI.Get_DataSourceDefinition(ObjID);
                    if (oDataSourceDedf.bOK && oDataSourceDedf.oResult != null)
                    {
                        oDataSourceD = (XIDataSource)oDataSourceDedf.oResult;
                    }
                    return oDataSourceD;
                case "structure":
                    List<XIDStructure> oStructD = new List<XIDStructure>();
                    XIDStructure oXIStruct = new XIDStructure();
                    var oStructDef = oXIStruct.GetXITreeStructure(ObjID, ObjName);
                    if (oStructDef.bOK && oStructDef.oResult != null)
                    {
                        oStructD = (List<XIDStructure>)oStructDef.oResult;
                    }
                    return oStructD;
                //case "xistructure":
                //    XIStructure oStrD = new XIStructure();
                //    XIDStructure oXIStr = new XIDStructure();
                //    var oStrDef = oXIStr.GetXIStructureDefinition(Convert.ToInt32(ObjID), ObjName);
                //    if (oStrDef.bOK && oStrDef.oResult != null)
                //    {
                //        oStrD = (XIStructure)oStrDef.oResult;
                //    }
                //    return oStrD;
                case "xiparamater":
                    XIParameter oXIParamD = new XIParameter();
                    var oXIParamDef = oDXI.Get_XIParameterDefinition(ObjID.ToString());
                    if (oXIParamDef.bOK && oXIParamDef.oResult != null)
                    {
                        oXIParamD = (XIParameter)oXIParamDef.oResult;
                    }
                    return oXIParamD;
                case "dialog":
                    XIDDialog xiDialog = new XIDDialog();
                    var oXiDialogDef = oDXI.Get_DialogDefinition(ObjID);
                    if (oXiDialogDef.bOK && oXiDialogDef.oResult != null)
                    {
                        xiDialog = (XIDDialog)oXiDialogDef.oResult;
                    }
                    return xiDialog;
                case "template":
                    List<XIContentEditors> oTemplateD = new List<XIContentEditors>();
                    var oXiTemplateDef = oDXI.Get_ContentDefinition(ObjID, ObjName);
                    if (oXiTemplateDef.bOK && oXiTemplateDef.oResult != null)
                    {
                        oTemplateD = (List<XIContentEditors>)oXiTemplateDef.oResult;
                    }
                    return oTemplateD;
                case "visualisation":
                    XIVisualisation oXIvisualisationD = new XIVisualisation();
                    var oXIvisualisationDef = oDXI.Get_VisualisationDefinition(ObjID, ObjName);
                    if (oXIvisualisationDef.bOK && oXIvisualisationDef.oResult != null)
                    {
                        oXIvisualisationD = (XIVisualisation)oXIvisualisationDef.oResult;
                    }
                    return oXIvisualisationD;
                case "structurecode":
                    XIDStructure oXIStructD = new XIDStructure();
                    var oXIStructDef = oDXI.Get_StructureDefinition(ObjID);
                    if (oXIStructDef.bOK && oXIStructDef.oResult != null)
                    {
                        oXIStructD = (XIDStructure)oXIStructDef.oResult;
                    }
                    return oXIStructD;
                case "refdata":
                    object oRefData = 0;
                    var oRefDataDef = oDXI.Get_AutoCompleteList(ObjID, ObjName);
                    if (oRefDataDef.bOK && oRefDataDef.oResult != null)
                    {
                        oRefData = oRefDataDef.oResult;
                    }
                    return oRefData;
                case "menu":
                    List<XIMenu> oMenuD = new List<XIMenu>();
                    var oXIMenuDef = oDXI.Get_RightMenuDefinition(ObjName);
                    if (oXIMenuDef.bOK && oXIMenuDef.oResult != null)
                    {
                        oMenuD = (List<XIMenu>)oXIMenuDef.oResult;
                    }
                    return oMenuD;
                //case "Step":
                //    var oStep = oQSRepo.GetStepDefinitionByID(ID, iUserID, sOrgName, sDatabase, sCurrentGuestUser);
                //    return oStep;
                //case "Layout":
                //    var oLayout = oQSRepo.GetLayoutByID(ID);
                //    return oLayout;
                case "doctypes":
                    XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                    var oXIDocTypesDef = oXIDocTypes.Get_FileDocTypes(ObjName,ObjID);
                    if (oXIDocTypesDef.bOK && oXIDocTypesDef.oResult != null)
                    {
                        oXIDocTypes = (XIInfraDocTypes)oXIDocTypesDef.oResult;
                    }
                    return oXIDocTypes;

                case "bogroup":
                    XIDGroup oXIGroup = new XIDGroup();
                    oXIGroup.ID = Convert.ToInt32(ObjID);
                    var oXIGroupDef = oXIGroup.Get_XIBOGroupDetails();
                    if (oXIGroupDef.bOK && oXIGroupDef.oResult != null)
                    {
                        oXIGroup = (XIDGroup)oXIGroupDef.oResult;
                    }
                    return oXIGroup;

                case "boscript":
                    XIDScript oXIScript = new XIDScript();
                    var oXIScriptDef = oXIScript.Get_XIBOScriptDetails(ObjID);
                    if (oXIScriptDef.bOK && oXIScriptDef.oResult != null)
                    {
                        oXIScript = (XIDScript)oXIScriptDef.oResult;
                    }
                    return oXIScript;
                case "inbox":
                    List<XIDInbox> oInboxD = new List<XIDInbox>();
                    var oInboxDef = oDXI.Get_InboxDefinition(ObjID);
                    if (oInboxDef.bOK && oInboxDef.oResult != null)
                    {
                        oInboxD = (List<XIDInbox>)oInboxDef.oResult;
                    }
                    return oInboxD;
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

        public XIIQS Set_QuestionSetCache(string ObjName, string sGUID, string ID, XIIQS oCacheobj)
        {
            object obj;
            HttpRuntime.Cache.Remove(ObjName + "_" + sGUID + "_" + ID);
            obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
            HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return oCacheobj;
        }

        public bool Set_ObjectSetCache(string SessionID, string keyName, string sGUID, object oCacheobj)
        {
            //object obj;
            HttpRuntime.Cache.Remove(keyName + "_" + sGUID + "_" + SessionID);
            //obj = HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID];
            HttpRuntime.Cache.Add(keyName + "_" + sGUID + "_" + SessionID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return true;
        }
        public object Get_ObjectSetCache(string keyName, string sGUID, string SessionID)
        {
            object obj = null;
            //XIIQS oCacheobj = new XIIQS();
            if (HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID] != null)
            {
                //HttpRuntime.Cache.Add(keyName + "_" + sGUID + "_" + SessionID, obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                obj = HttpRuntime.Cache[keyName + "_" + sGUID + "_" + SessionID];
            }
            return obj;
        }

        public XIIQS Get_QuestionSetCache(string ObjName, string sGUID, int ID)
        {
            object obj;
            XIIQS oCacheobj = new XIIQS();
            if (HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID] == null)
            {
                HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            else
            {
                obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
                oCacheobj = (XIIQS)obj;
                return oCacheobj;
            }
            Get_QuestionSetCache(ObjName, sGUID, ID);
            return oCacheobj;
        }
        //public XiLinks GetXiLinkDetails(int XiLinkID, string sGUID, int iUserID, string sOrgName, string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    XiLinks oXilink = new XiLinks();
        //    oXilink = (XiLinks)GetObjectFromCache(XIConstant.CacheXILink, sGUID, iUserID, sOrgName, sDatabase, XiLinkID);
        //    return oXilink;
        //}    

        public string CacheKeyBuilder(string sObjType, string sName, string ID)
        {
            string sCacheKey = string.Empty;
            var sAppName = Singleton.Instance.sAppName;
            if (!string.IsNullOrEmpty(sAppName))
            {
                sCacheKey = sCacheKey + sAppName + "_";
            }
            sCacheKey = sCacheKey + "Definition_";
            if (!string.IsNullOrEmpty(sObjType))
            {
                sCacheKey = sCacheKey + sObjType + "_";
            }
            if (!string.IsNullOrEmpty(sName))
            {
                sCacheKey = sCacheKey + sName + "_";
            }
            if (!string.IsNullOrEmpty(ID) && ID != "0")
            {
                sCacheKey = sCacheKey + ID + "_";
            }
            if (!string.IsNullOrEmpty(sCacheKey))
            {
                sCacheKey = sCacheKey.Substring(0, sCacheKey.Length - 1);
            }
            return sCacheKey;
        }

        public CResult MergeXILinkParameters(XILink oXiLink, string sGUID = "", List<CNV> oParams = null, string sSessionID = "")
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
                //string sSessionID = HttpContext.Current.Session.SessionID;
                if (oXiLink != null && oXiLink.XiLinkNVs != null && oXiLink.XiLinkNVs.Count() > 0)
                {
                    foreach (var param in oXiLink.XiLinkNVs)
                    {
                        if (!string.IsNullOrEmpty(param.Value))
                        {
                            CResult oCRes = new CResult();
                            oCRes = ResolveMe(param.Value, sSessionID, sGUID);
                            if (oCRes.bOK)
                            {
                                string sResolvedValue = string.Empty;
                                sResolvedValue = (string)oCRes.oResult;
                                Set_ParamVal(sSessionID, sGUID, null, param.Name, sResolvedValue, null, null);
                            }
                        }
                    }
                }
                if (oParams != null && oParams.Count() > 0)
                {
                    foreach (var param in oParams)
                    {
                        if (!string.IsNullOrEmpty(param.sValue))
                        {
                            CResult oCRes = new CResult();
                            oCRes = ResolveMe(param.sValue, sSessionID, sGUID);
                            if (oCRes.bOK)
                            {
                                string sResolvedValue = string.Empty;
                                sResolvedValue = (string)oCRes.oResult;
                                Set_ParamVal(sSessionID, sGUID, null, param.sName, sResolvedValue, null, null);
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xilink params into cache. XILinkID is " + oXiLink.XiLinkID });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public CResult AddParamsToGUID(int iParameterID, string sGUID)
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
                XIParameter oXIParam = null;
                if (iParameterID > 0)
                {
                    oXIParam = (XIParameter)GetObjectFromCache(XIConstant.CacheXIParamater, null, iParameterID.ToString());
                    if (oXIParam != null)
                    {
                        string sSessionID = HttpContext.Current.Session.SessionID;
                        if (oXIParam.XiParameterNVs != null && oXIParam.XiParameterNVs.Count() > 0)
                        {
                            foreach (var param in oXIParam.XiParameterNVs)
                            {
                                if (!string.IsNullOrEmpty(param.Value))
                                {
                                    CResult oCRes = new CResult();
                                    oCRes = ResolveMe(param.Value, sSessionID, sGUID);
                                    if (oCRes.bOK)
                                    {
                                        string sResolvedValue = string.Empty;
                                        sResolvedValue = (string)oCRes.oResult;
                                        Set_ParamVal(sSessionID, sGUID, null, param.Name, sResolvedValue, null, null);
                                    }
                                }
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache: XIParameter ID is " + iParameterID });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult; // always

            //return oURL;
        }

        public List<CNV> ResolveParameters(List<CNV> Params, string sSessionID, string sGUID)
        {
            if (!string.IsNullOrEmpty(sGUID))
            {
                foreach (var param in Params)
                {
                    string sResolvedValue = string.Empty;
                    CResult oCR = new CResult();
                    oCR = ResolveMe(param.sValue, sSessionID, sGUID);
                    if (oCR.bOK)
                    {
                        sResolvedValue = (string)oCR.oResult;
                        Params.Where(m => m.sName.ToLower() == param.sName.ToLower()).FirstOrDefault().sValue = sResolvedValue;
                    }
                }
            }
            return Params;
        }

        public CResult ResolveMe(string sParam, string sSessionID, string sGUID)
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
                string sValue = string.Empty;
                Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(sParam);
                if (sParam.StartsWith("xi."))
                {
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sParam.ToString();
                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sValue = (string)oCR.oResult;
                    }
                }
                else if (sParam.StartsWith("xi.s") || sParam.Contains("xi.r"))
                {
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sParam.ToString();
                    oCR = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sValue = (string)oCR.oResult;
                    }
                }
                else if (sParam.StartsWith("{-"))
                {
                    sValue = Get_ParamVal(sSessionID, sGUID, null, sParam);
                }
                else if (sParam.StartsWith("xic."))
                {
                    XIAPI oAPI = new XIAPI();
                    var split = sParam.Split('.').ToList();
                    if (split[1].ToLower() == "iuserid")
                    {
                        var oUser = oAPI.GetUserID();
                        if (oUser.bOK && oUser.oResult != null)
                        {
                            sValue = ((int)oUser.oResult).ToString();
                        }
                    }
                }
                else if (matches.Count > 0)
                {
                    foreach (var match in matches)
                    {
                        if (match.ToString().Contains('|'))
                        {
                            var SplitPipe = match.ToString().Split('|').ToList();
                            if (SplitPipe != null && SplitPipe.Count() > 0)
                            {
                                if (SplitPipe[0].ToLower() == "xip")
                                {
                                    var Prm = "{" + match.ToString() + "}";
                                    sValue = Get_ParamVal(sSessionID, sGUID, null, Prm);
                                }
                                else if (SplitPipe[0].ToLower() == "xir")
                                {

                                }
                                else if (SplitPipe[0].ToLower() == "xis")
                                {
                                    XIDScript oXIScript = new XIDScript();
                                    oXIScript.sScript = match.ToString();
                                    oXIScript.Execute_Script(sGUID, sSessionID);
                                }
                            }
                        }
                    }
                }
                else
                {
                    sValue = sParam;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = sValue;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while merging xiparameter params into cache" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        #endregion CacheObject
        public string GetParentGUIDRecurrsive(string sGUID)
        {
            sParGUID = sGUID;
            var sParentGUID = Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
            if (!string.IsNullOrEmpty(sParentGUID))
            {
                GetParentGUIDRecurrsive(sParentGUID);
            }
            else
            {
                return sParGUID;
            }
            return sParGUID;
        }

    }
}