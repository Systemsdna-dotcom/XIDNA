using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XIDNA.Repository;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Common
{
    public class FormComponent
    {
        BusinessObjectsRepository OBORepo = new BusinessObjectsRepository();

        public string sBOName { get; set; }
        public string sGroupName { get; set; }
        public string sLockGroup { get; set; }
        public int iInstanceID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sDisplayMode { get; set; }

        public cBODisplay XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            sDisplayMode = oParams.Where(m => m.sName == "DisplayMode").Select(m => m.sValue).FirstOrDefault();
            cBODisplay oBODisplay = new cBODisplay();
            //First set all properties by extracting from oParams

            sGroupName = oParams.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
            sLockGroup = oParams.Where(m => m.sName.ToLower() == "LockGroup".ToLower()).Select(m => m.sValue).FirstOrDefault();
            if (oParams.Where(m => m.sName.ToLower() == "BO".ToLower()).FirstOrDefault() != null)
            {
                if (!oParams.Where(m => m.sName.ToLower() == "BO".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                {
                    sBOName = oParams.Where(m => m.sName == "BO").Select(m => m.sValue).FirstOrDefault();
                }
            }
            var WrapperParms = new List<cNameValuePairs>();
            var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam1".ToLower())).ToList();
            if (WatchParam.Count() > 0)
            {
                foreach (var items in WatchParam)
                {
                    if (!string.IsNullOrEmpty(items.sValue))
                    {
                        var Prams = oXIAPI.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                        if (Prams != null)
                        {
                            WrapperParms = Prams.nSubParams;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(WatchParam.FirstOrDefault().sValue))
            {
                if ( WrapperParms != null && WrapperParms.Count() > 0)
                {
                    var ActiveBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault(); ;// oXIAPI.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");
                    var Prm = "{XIP|" + ActiveBO + ".id}";
                    var XIPBOID = WrapperParms.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault(); ; // oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (XIPBOID != null)
                    {
                        iInstanceID = Convert.ToInt32(XIPBOID);
                    }
                    else
                    {
                        iInstanceID = 0;
                    }
                    if (sBOName == null && !(string.IsNullOrEmpty(ActiveBO)))
                    {
                        sBOName = ActiveBO;
                    }
                }
                else
                {
                    iInstanceID = 0;
                }
            }
            else
            {
                var ActiveBO = oXIAPI.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");// oParams.Where(m => m.sName.ToLower() == "{XIP|ActiveBO}".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var Prm = "{XIP|" + ActiveBO + ".id}";
                var XIPBOIID = oXIAPI.Get_ParamVal(sSessionID, sGUID, null, Prm); //oParams.Where(m => m.sName.ToLower() == Prm.ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (XIPBOIID != null)
                {
                    iInstanceID = Convert.ToInt32(XIPBOIID);
                }
                else
                {
                    iInstanceID = 0;
                }
                if (sBOName == null && !(string.IsNullOrEmpty(ActiveBO)))
                {
                    sBOName = ActiveBO;
                }
            }

            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
            if (!string.IsNullOrEmpty(sBOName) && !sBOName.StartsWith("{XIP|"))
            {
                oBODisplay = oXIAPI.GetFormData(sBOName, sGroupName, iInstanceID, string.Empty, iUserID, sOrgName, sDatabase, null);
                if (!string.IsNullOrEmpty(sLockGroup))
                {
                    var GroupFields = oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                    if (!string.IsNullOrEmpty(GroupFields))
                    {
                        var oGrpFields = GroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        oBODisplay.BOInstance.NVPairs.Where(m => oGrpFields.Any(n => n == m.sName)).ToList().ForEach(c => c.bLock = true);
                        oBODisplay.BOInstance.NVPairs.Where(m => m.sName.ToLower() == "id").ToList().ForEach(m => m.bLock = true);
                    }
                }
                if (!string.IsNullOrEmpty(sDisplayMode))
                {
                    cVisualisations oVis = new cVisualisations();
                    oVis.sName = "DisplayMode";
                    oVis.sValue = sDisplayMode;
                    oBODisplay.Visualisations = new List<cVisualisations>();
                    oBODisplay.Visualisations.Add(oVis);
                }
            }
            return oBODisplay;
        }
    }

    public class TreeComponent
    {
        XIComponentsRepository oXIComRepo = new XIComponentsRepository();
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public List<cTreeView> XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            var BOLevels = oParams.Where(m => m.sName.ToLower().Contains("BOLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            var FKs = oParams.Where(m => m.sName.ToLower().Contains("FKLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            List<cNameValuePairs> oTreeParams = new List<cNameValuePairs>();
            oTreeParams.AddRange(BOLevels);
            oTreeParams.AddRange(FKs);
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            var Nodes = oXIComRepo.GetTreeStructure(oTreeParams, iUserID, sOrgName, sDatabase);
            return Nodes;
        }
    }


    public class OneClickComponent
    {
        XiLinkRepository oXIRepo = new XiLinkRepository();
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public int OneClickID { get; set; }
        public string sOrgName { get; set; }
        public string sContext { get; set; }
        public int iSectionInstanceID { get; set; }
        public int iStepInstanceID { get; set; }
        public string AttributeName1 { get; set; }
        public string AttributeValue1 { get; set; }
        public string AttributeName2 { get; set; }
        public string AttributeValue2 { get; set; }
        public string sSection { get; set; }
        public string sDisplayMode { get; set; }
        public VMResultList XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            sDisplayMode = oParams.Where(m => m.sName == "DisplayMode").Select(m => m.sValue).FirstOrDefault();
            var WrapperParms = new List<cNameValuePairs>();
            var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
            if (WatchParam.Count() > 0)
            {
                foreach (var items in WatchParam)
                {
                    if (!string.IsNullOrEmpty(items.sValue))
                    {
                        var Prams = oXIAPI.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                        if (Prams != null)
                        {
                            WrapperParms = Prams.nSubParams;
                        }
                    }
                }
            }
            List<cNameValuePairs> nParams = new List<cNameValuePairs>();
            sContext = oParams.Where(m => m.sName == "Context").Select(m => m.sValue).FirstOrDefault();
            sSection = oParams.Where(m => m.sName == "Section").Select(m => m.sValue).FirstOrDefault();
            if (oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
            {
                OneClickID = Convert.ToInt32(WrapperParms.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault());
            }
            else if (oParams.Where(m => m.sName == "1ClickID").FirstOrDefault() != null)
            {
                OneClickID = Convert.ToInt32(oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault());
            }
            else
            {
                OneClickID = 0;
            }
            AttributeName1 = oParams.Where(m => m.sName == "AttributeName1").Select(m => m.sValue).FirstOrDefault();
            AttributeValue1 = oParams.Where(m => m.sName == "AttributeValue1").Select(m => m.sValue).FirstOrDefault();
            if (AttributeValue1 != null && AttributeValue1.ToLower() == "{qsinstance}")
            {
                AttributeValue1 = oParams.Where(m => m.sName == "FKiQSInstanceID").Select(m => m.sValue).FirstOrDefault();
            }
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
            if (AttributeValue1 != null)
            {
                nParams.Add(new cNameValuePairs { sName = AttributeName1, sValue = AttributeValue1 });
                nParams.Add(new cNameValuePairs { sName = "Section", sValue = sSection });
            }

            nParams.AddRange(oParams.Where(m => m.sType == "Structure").ToList());
            VMResultList Result = new VMResultList();
            if (OneClickID > 0)
            {
                Result = oXIRepo.GetHeadingsForList(OneClickID, null, sDatabase, 0, iUserID, sOrgName);
                Result.ReportID = OneClickID;
                Result.sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sDisplayMode))
                {
                    Result.ActionType = "View";
                }
            }
            return Result;
        }
    }

    public class FullAddressComponent
    {
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sBOName { get; set; }
        public string sGroupName { get; set; }
        public string sOrgName { get; set; }
        public int iInstanceID { get; set; }
        public string sContext { get; set; }
        public int FKiSectionInstanceID { get; set; }
        public int FKiStepInstanceID { get; set; }
        public string sSection { get; set; }
        BusinessObjectsRepository OBORepo = new BusinessObjectsRepository();
        CXiAPI oXIAPI = new CXiAPI();
        public void XISave(cBOInstance oBOInstance, string sGUID, string sContext, string sDatabase, int iUserID, string sOrgName)
        {
            var Response = oXIAPI.SaveFormData(oBOInstance, sGUID, sContext, sDatabase, iUserID, sOrgName);
        }

        public cBODisplay XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            //Dan said to change XILoad Name to XIAPI and add XIMethod parameters to this method
            //case XImethod.tolower()
            //XILoad And XIsave
            List<cNameValuePairs> nParams = new List<cNameValuePairs>();
            cBODisplay oBODisplay = new cBODisplay();
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sBOName = oParams.Where(m => m.sName == "BO").Select(m => m.sValue).FirstOrDefault();
            sGroupName = oParams.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
            sContext = oParams.Where(m => m.sName == "Context").Select(m => m.sValue).FirstOrDefault();
            FKiSectionInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "FKiSectionInstanceID").Select(m => m.sValue).FirstOrDefault());
            FKiStepInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "FKiStepInstanceID").Select(m => m.sValue).FirstOrDefault());
            sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
            nParams.Add(new cNameValuePairs { sName = "Context", sValue = sContext });
            nParams.Add(new cNameValuePairs { sName = "FKiStepInstanceID", sValue = FKiStepInstanceID.ToString() });
            nParams.Add(new cNameValuePairs { sName = "FKiSectionInstanceID", sValue = FKiSectionInstanceID.ToString() });

            if (!string.IsNullOrEmpty(sBOName))
            {
                oXIAPI = new CXiAPI();
                oBODisplay = oXIAPI.GetFormData(sBOName, sGroupName, iInstanceID, string.Empty, iUserID, sOrgName, sDatabase, nParams);
            }
            return oBODisplay;
        }
    }

    public class ImageComponent
    {
        public cImageComponent XIAPI(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            cImageComponent oComponent = new cImageComponent();
            oComponent.iXILinkID = Convert.ToInt32(oParams.Where(m => m.sName == "XILinkID").Select(m => m.sValue).FirstOrDefault());
            oComponent.sImage = oParams.Where(m => m.sName == "Image").Select(m => m.sValue).FirstOrDefault();
            return oComponent;
        }
    }

    public class QuestionSetComponent
    {
        public int iBODID { get; set; }
        public string sMode { get; set; }
        public string sStructureCode { get; set; }
        public string sQSName { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        XIComponentsRepository oXIComRepo = new XIComponentsRepository();
        public cQSDefinition XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            iBODID = Convert.ToInt32(oParams.Where(m => m.sName == "BODID").Select(m => m.sValue).FirstOrDefault());
            if (iBODID == 0)
            {
                iBODID = Convert.ToInt32(oXIAPI.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}"));
            }
            sQSName = oParams.Where(m => m.sName == "QSFixedTemplate").Select(m => m.sValue).FirstOrDefault();
            sMode = oParams.Where(m => m.sName == "Mode").Select(m => m.sValue).FirstOrDefault();
            sStructureCode = oParams.Where(m => m.sName == "StructureCode").Select(m => m.sValue).FirstOrDefault();
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
            var oQSDefintion = oXIComRepo.GetQuestionSetComponent(iBODID, sStructureCode, sMode, sQSName, iUserID, sOrgName, sDatabase);
            return oQSDefintion;
        }
    }
    public class XITreeStructureComponent
    {
        XIComponentsRepository oXIComRepo = new XIComponentsRepository();
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCode { get; set; }
        public int iBODID { get; set; }
        public string sMode { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public List<cXIStructure> XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            var WrapperParms = new List<cNameValuePairs>();
            var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam1".ToLower())).ToList();
            if (WatchParam.Count() > 0)
            {
                foreach (var items in WatchParam)
                {
                    if (!string.IsNullOrEmpty(items.sValue))
                    {
                        var Prams = oXIAPI.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                        if (Prams != null)
                        {
                            WrapperParms = Prams.nSubParams;
                        }
                    }
                }
            }
            iBODID = Convert.ToInt32(oParams.Where(m => m.sName == "iBODID").Select(m => m.sValue).FirstOrDefault());
            sCode = oParams.Where(m => m.sName == "sCode").Select(m => m.sValue).FirstOrDefault();
            sMode = oParams.Where(m => m.sName == "sMode").Select(m => m.sValue).FirstOrDefault();
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            var Nodes = oXIComRepo.GetXITreeStructure(iBODID, sCode, iUserID, sOrgName, sDatabase);
            if (Nodes.Count() > 0)
            {
                Nodes.ToList().ForEach(m => m.sMode = sMode);
            }
            if(WrapperParms !=null && WrapperParms.Count()> 0)
            {
                var ActiveBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = WrapperParms.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                if(!string.IsNullOrEmpty(ActiveBO) && !string.IsNullOrEmpty(iBOIID))
                {
                    var sNameAttribute = oXIAPI.GetBONameAttributeValue(ActiveBO, Convert.ToInt32(iBOIID), iUserID, sDatabase);
                    if (!string.IsNullOrEmpty(sNameAttribute))
                    {
                        Nodes.Where(m => m.sBO == ActiveBO).FirstOrDefault().sName = Nodes.Where(m => m.sBO == ActiveBO).FirstOrDefault().sName + " (" + sNameAttribute + ")";
                        Nodes.Where(m => m.sBO == ActiveBO).FirstOrDefault().sInsID = iBOIID;
                    }                    
                }
            }
            return Nodes;
        }
    }

    public class QSComponent
    {
        public int iQSDID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCurrentUserGUID { get; set; }
        public string sGUID { get; set; }
        XiLinkRepository oXIRepo = new XiLinkRepository();
        public cQSInstance XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            iQSDID = Convert.ToInt32(oParams.Where(m => m.sName == "iQSDID").Select(m => m.sValue).FirstOrDefault());
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            sCurrentUserGUID = oParams.Where(m => m.sName == "sCurrentUserGUID").Select(m => m.sValue).FirstOrDefault();
            cQSInstance oQSInstance = new cQSInstance();
            if (iQSDID > 0)
            {
                oQSInstance = oXIRepo.GetQuestionSetInstance(iQSDID, sGUID, null, 0, 0, iUserID, sOrgName, sDatabase, sCurrentUserGUID);
            }
            return oQSInstance;
        }
    }

    public class StepComponent
    {
        public int iStepDID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCurrentUserGUID { get; set; }
        public string sGUID { get; set; }
        XiLinkRepository oXIRepo = new XiLinkRepository();
        public cQSStepDefiniton XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            if (oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).FirstOrDefault() != null)
            {
                if (oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                {
                    iStepDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "{XIP|iStepDID}".ToLower()).Select(m => m.sValue).FirstOrDefault());
                }
                else
                {
                    iStepDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).Select(m => m.sValue).FirstOrDefault());
                }
            }
            //iStepDID = Convert.ToInt32(oParams.Where(m => m.sName == "iStepDID").Select(m => m.sValue).FirstOrDefault());
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
            sCurrentUserGUID = oParams.Where(m => m.sName == "sCurrentUserGUID").Select(m => m.sValue).FirstOrDefault();
            cQSStepDefiniton oQSStep = new cQSStepDefiniton();
            if (iStepDID > 0)
            {
                oQSStep = oXIRepo.GetStepDefinitionByID(iStepDID, iUserID, sOrgName, sDatabase, sCurrentUserGUID);
                //if (oQSStep.Layout != null)
                //{
                //    oQSStep.Layout.sGUID = sGUID;
                //}
            }
            return oQSStep;
        }
    }
    #region BOComponent
    public class BOComponent
    {
        XIComponentsRepository oXIComRepo = new XIComponentsRepository();
        public int iBODID { get; set; }
        public int iBOIID { get; set; }
        public string sGroupName { get; set; }
        public string sLoadGroup { get; set; }
        public string sLockGroup { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }

        public cBODisplay XILoad(List<cNameValuePairs> oParams, CXiAPI oXIAPI)
        {
            cBODisplay oBODisplay = new cBODisplay();
            //First set all properties by extracting from oParams
            iBODID = Convert.ToInt32(oParams.Where(m => m.sName == "BODID").Select(m => m.sValue).FirstOrDefault());
            iBOIID = Convert.ToInt32(oParams.Where(m => m.sName == "BOIID").Select(m => m.sValue).FirstOrDefault());
            sGroupName = oParams.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
            sLoadGroup = oParams.Where(m => m.sName == "LoadGroup").Select(m => m.sValue).FirstOrDefault();
            sLockGroup = oParams.Where(m => m.sName == "LockGroup").Select(m => m.sValue).FirstOrDefault();
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
            if (iBODID != 0 && iBOIID != 0)
            {
                oBODisplay = oXIComRepo.GetBOComponentData(iBODID, iBOIID, sGroupName, sLoadGroup, sLockGroup, iUserID, sOrgName, sDatabase);
            }
            return oBODisplay;
        }
    }
    #endregion BOComponent
}