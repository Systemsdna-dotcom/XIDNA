using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XISystem;
using Newtonsoft.Json;

namespace XIInfrastructure
{
    public class XIInfraOneClickComponent : XIDefinitionBase
    {

        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int OneClickID;
        public string sOrgName { get; set; }
        public string sDisplayMode { get; set; }
        public string sStructureName { get; set; }
        public string sCondition { get; set; }
        XIInfraCache oCache = new XIInfraCache();
        XIDStructure oXIDStructure = new XIDStructure();
        public CResult XILoad(List<CNV> oParams)
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
            try
            {
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName == "StructureName").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                sDisplayMode = oParams.Where(m => m.sName == "DisplayMode").Select(m => m.sValue).FirstOrDefault();
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
                if (WatchParam.Count() > 0)
                {
                    foreach (var items in WatchParam)
                    {
                        if (!string.IsNullOrEmpty(items.sValue))
                        {
                            var Prams = oCache.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                            if (Prams != null)
                            {
                                WrapperParms = Prams.nSubParams;
                            }
                        }
                    }
                }
                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    string sOneClickID = WrapperParms.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                    var sParentFK = WrapperParms.Where(m => m.sName == "ParentFKColumn").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sParentFK))
                    {
                        var sParentInsID = WrapperParms.Where(m => m.sName == "ParentInsID").Select(m => m.sValue).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sParentInsID))
                        {
                            sCondition = sParentFK + "=" + sParentInsID;
                        }
                        else
                        {
                            sCondition = sParentFK + "=-1";
                        }
                    }
                }
                //if (oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault() != null)
                //{
                //    if (WrapperParms != null && WrapperParms.Count() > 0)
                //    {
                //        OneClickID = Convert.ToInt32(WrapperParms.Where(m => m.sName == "i1ClickID").Select(m => m.sValue).FirstOrDefault());
                //    }
                //    else
                //    {
                //        OneClickID = Convert.ToInt32(oParams.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault());
                //    }
                //}
                else if (oParams.Where(m => m.sName == "1ClickID").FirstOrDefault() != null)
                {
                    string sOneClickID = oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault();
                    if (int.TryParse(sOneClickID, out OneClickID))
                    {

                    }
                    else
                    {
                        OneClickID = 0;
                    }
                }
                else
                {
                    OneClickID = 0;
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XID1Click o1ClickD = new XID1Click();
                XID1Click o1ClickC = new XID1Click();
                if (OneClickID > 0)
                {

                    //Get 1-Click Defintion             
                    o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());
                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                    //Get BO Definition
                    XIDBO oBOD = new XIDBO();
                    if (o1ClickC.BOIDXIGUID != null && o1ClickC.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickC.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickC.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickC.BOID.ToString());
                    }
                    o1ClickC.BOD = oBOD;
                    o1ClickC.sBOName = oBOD.Name;
                    if (!string.IsNullOrEmpty(sCondition))
                    {
                        o1ClickC.SearchText = sCondition;
                    }
                    //Get Headings of 1-Click
                    o1ClickC.Get_1ClickHeadings();
                    if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Grid.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()))
                    {
                        XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                        List<CNV> nParms = new List<CNV>();
                        nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                        o1ClickC.ReplaceFKExpressions(nParms);
                        o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                        //if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()) || o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.List.ToString()))
                        //{
                        //    o1ClickC.bIsResolveFK = true;
                        //}                                                
                        Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Execute();
                        string sButtons = string.Empty;
                        string sCheckBox = string.Empty;
                        string sEditBtn = string.Empty;
                        string sCopyBtn = string.Empty;
                        string sDeleteBtn = string.Empty;
                        string sViewbtn = string.Empty;
                        //if (o1ClickC.bIsEdit)
                        //{
                        //    sEditBtn = "<input type='button' class='btn btn-theme' value='Edit' onclick ='fncEditBO(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        //}
                        if (o1ClickC.bIsCopy)
                        {
                            sCopyBtn = "<input type='button' class='btn btn-theme' value='Copy' onclick ='fncCopyBO(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        }
                        if (o1ClickC.bIsDelete)
                        {
                            sDeleteBtn = "<input type='button' class='btn btn-theme' value='Delete' onclick ='fncDeleteBO(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        }
                        if (o1ClickC.bIsView)
                        {
                            sViewbtn = "<input type='button' class='btn lbluebtn' value='View' onclick ='fncViewBO(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        }
                        if (o1ClickC.bIsCheckbox) // * TO DO THIS IS HARD CODED FOR RECONCILLIATION NEED TO CHANGE IT LATER
                        {
                            sCheckBox = "<input type='checkbox' class='chkReconcilliation' Onchange ='fncCheckboxOnchange(this," + o1ClickC.ID + "," + o1ClickC.CreateGroupID + "," + o1ClickC.BOID + "," + o1ClickC.iCreateXILinkID + ")' />";
                        }
                        sButtons = sEditBtn + sCopyBtn + sDeleteBtn + sViewbtn;
                        o1ClickC.oDataSet.ToList().ForEach(m =>
                        {
                            if (!string.IsNullOrEmpty(sButtons))
                            {
                                XIIAttribute oAttrI = new XIIAttribute();
                                oAttrI.sName = "Actions";
                                oAttrI.sValue = sButtons;
                                m.Value.Attributes["Actions"] = oAttrI;
                            }
                            if (!string.IsNullOrEmpty(sCheckBox))
                            {
                                Dictionary<string, XIIAttribute> nAttributes = new Dictionary<string, XIIAttribute>();
                                XIIAttribute oAttrI = new XIIAttribute();
                                oAttrI.sName = "Select";
                                oAttrI.sValue = sCheckBox;
                                //m.Value.Attributes["Actions"] = oAttrI;
                                nAttributes["Select"] = oAttrI;
                                //m.Value.Attributes.Intersect(0, "Actions", oAttrI);
                                foreach (var item in m.Value.Attributes)
                                {
                                    nAttributes[item.Key] = item.Value;
                                }
                                m.Value.Attributes = nAttributes;
                            }
                        });
                        foreach (var oInstance in o1ClickC.oDataSet.Values)
                        {
                            var oFileAttrs = oBOD.Attributes.Values.Where(m => m.FKiFileTypeID > 0).ToList();
                            if (!o1ClickC.bIsMultiBO)
                            {
                                List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                                foreach (var oAttr in oFileAttrs)
                                {
                                    var sName = oAttr.Name;
                                    var sFileID = oInstance.Attributes.Values.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(sFileID))
                                    {
                                        List<XIDropDown> sPDFPathDetails = new List<XIDropDown>();
                                        var NewFileID = sFileID.Split(',').ToList();
                                        foreach (var item in NewFileID)
                                        {
                                            if (!string.IsNullOrEmpty(item.ToString()))
                                            {
                                                XIInfraDocs oDocs = new XIInfraDocs();
                                                oDocs.ID = Convert.ToInt32(item);
                                                var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                                if (sImagePathDetails != null)
                                                {
                                                    ImagePathDetails.AddRange(sImagePathDetails);
                                                }
                                            }
                                        }
                                    }
                                    oInstance.Attributes.Values.Where(x => x.sName.ToLower() == sName.ToLower()).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                                }
                            }

                        }
                        if (o1ClickC.DisplayAs == (Int32)Enum.Parse(typeof(xiEnumSystem.xi1ClcikDisplayAS), xiEnumSystem.xi1ClcikDisplayAS.Repeater.ToString()))
                        {
                            if (o1ClickC.FKiComponentID > 0 && o1ClickC.XIComponent != null)
                            {
                                if (o1ClickC.XIComponent.sName.ToLower() == XIConstant.HTMLComponent.ToLower())
                                {
                                    string iLayoutID = string.Empty;
                                  var iTemplateID = string.Empty;
                                    if (o1ClickC.XIComponent.Params != null && o1ClickC.XIComponent.Params.Count() > 0)
                                    {
                                        iLayoutID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "slayout").Select(m => m.sValue).FirstOrDefault();
                                        iTemplateID = o1ClickC.XIComponent.Params.Where(m => m.sName.ToLower() == "stemplate").Select(m => m.sValue).FirstOrDefault();
                                    }
                                    XIContentEditors oContent = new XIContentEditors();
                                    List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                                    if (!string.IsNullOrEmpty(iLayoutID))
                                    {
                                        var oLayout = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, iLayoutID);
                                        oContent = new XIContentEditors();
                                        oContent.Content = oLayout.LayoutCode;
                                    }
                                    else if (!string.IsNullOrEmpty(iTemplateID))
                                    {
                                        oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, iTemplateID);
                                        if (oContentDef != null && oContentDef.Count() > 0)
                                        {
                                            oContent = oContentDef.FirstOrDefault();
                                        }
                                    }
                                    o1ClickC.RepeaterResult = new List<string>();
                                    foreach (var oBOI in oRes.Values)
                                    {
                                        var disable = "unchecked";
                                        var oCacheI = oCache.Get_XICache();
                                        var oCacheAddonI = oCacheI.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sGUID).NMyInstance.Where(m => m.Key.Contains("Addon_")).Select(m => m.Value.sValue).ToList();
                                        if (oBOI != null && oBOI.Attributes != null)
                                        {
                                            if (oBOI.Attributes.ContainsKey("id"))
                                            {
                                                if (oCacheAddonI.Contains(oBOI.Attributes["id"].sValue))
                                                {
                                                    disable = "checked";
                                                }
                                            }
                                        }
                                        List<XIIBO> nBOI = new List<XIIBO>();
                                        if (oBOI.iBODID == 0)
                                        {
                                            oBOI.iBODID = oBOD.BOID;
                                        }
                                        nBOI.Add(oBOI);
                                        var sInstanceID = oBOI.AttributeI(oBOD.sPrimaryKey).sValue;
                                        CResult Result = new CResult();
                                        XIContentEditors oConent = new XIContentEditors();
                                        oConent.sSessionID = sSessionID;
                                        if (!string.IsNullOrEmpty(sStructureName))
                                        {
                                            XIIXI oIXI = new XIIXI();
                                            var oStructobj = oIXI.BOI(oBOD.Name, sInstanceID).Structure(sStructureName).XILoad(null, true);
                                            //Result = oConent.MergeTemplateContent(oContent, oStructobj);
                                            Result = oConent.MergeContentTemplate(oContent, oStructobj);
                                        }
                                        else
                                        {
                                            XIBOInstance oBOIns = new XIBOInstance();
                                            oBOIns.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                                            oBOIns.oStructureInstance[oBOD.Name.ToLower()] = nBOI;
                                            //Result = oConent.MergeTemplateContent(oContent, oBOIns);
                                            Result = oConent.MergeContentTemplate(oContent, oBOIns);
                                        }
                                        if (Result.bOK && Result.oResult != null)
                                        {
                                            var oBOIHTML = Result.oResult.ToString();
                                            var regx = new Regex("{.*?}");
                                            var Matches = regx.Matches(oBOIHTML);
                                            if (Matches.Count > 0)
                                            {
                                                foreach (var match in Matches)
                                                {
                                                    if (match.ToString().ToLower() == "{editbtn}")
                                                    {
                                                        var EditBtn = "<input type=\"button\" value=\"Edit\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                        oBOIHTML = oBOIHTML.Replace("{EditBtn}", EditBtn);
                                                    }
                                                    else if (match.ToString().ToLower() == "{deletebtn}")
                                                    {
                                                        var DeleteBtn = "<input type=\"button\" value=\"Delete\" class=\"btn btn-theme\" id=\"Delete_" + oBOD.Name + "_" + sInstanceID + "\" data-id=" + sInstanceID + " onclick=\"DeleteBO('" + oBOI.Attributes[oBOD.sPrimaryKey].sValue + "','" + sGUID + "','" + oBOD.Name + "','" + o1ClickC.XIComponent.sName.ToLower() + "' ,this " + " )\" />";
                                                        oBOIHTML = oBOIHTML.Replace("{DeleteBtn}", DeleteBtn);
                                                    }
                                                    else if (match.ToString().ToLower() == "{more details}" || match.ToString().ToLower() == "{view quote}")
                                                    {
                                                        List<CNV> fncParams = new List<CNV>();
                                                        fncParams.Add(new CNV { sName = "{-iInstanceID}", sValue = sInstanceID });
                                                        fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                        System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                        string sJSON = oSerializer.Serialize(fncParams);
                                                        //string sInputValue = Regex.Replace(match.ToString().Replace("{", "").Replace("}", ""), @"\s+", "");
                                                        var MoreDetailsBtn = "<input type=\"button\" onclick=XILinkLoadJson(" + o1ClickC.RowXiLinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                        //var MoreDetailsBtn = "<input type=\"button\" value=\""+ match.ToString().Replace("{", "").Replace("}", "") + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"ViewMore('"+ sInputValue + "_" + sInstanceID + "')\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), MoreDetailsBtn);
                                                    }
                                                    else if (match.ToString().ToLower() == "{remove}" || match.ToString().ToLower() == "{add}")
                                                    {
                                                        var RemoveBtn = "<input type=\"checkbox\" class=\"switch-input\" Onchange =\"fnc1clickremove('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + sInstanceID + "' ,this " + ")\" " + disable + " />";
                                                        oBOIHTML = oBOIHTML.Replace("{Remove}", RemoveBtn);
                                                    }
                                                    //else if (match.ToString().ToLower() == "{add}")
                                                    //{
                                                    //    string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                    //    var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();
                                                    //    //var Btn = "<input type=\"button\" value=\"Add\" class=\"btn btn-theme\" id=\"Add_" + oBOD.Name + "_" + sInstanceID + "\"  data-id=" + sInstanceID + " onclick=\"fnc1clickcreate('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + oLink.FKiXILinkID + "','" + sInstanceID + "')\"  " + disable + " />";
                                                    //    var Btn = "<input type=\"button\" value=\"Add\" class=\"btn btn-sm btn-success table-btn ConvetBtn\" data-type=\"notselected\" id=\"Add_" + oBOD.Name + "_" + sInstanceID + "\"  data-id=" + sInstanceID + " onclick=\"fnAddon('" + sInstanceID + "')\"  " + disable + " />";
                                                    //    oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                    //}
                                                    else if (match.ToString().ToLower() == "{buynow}")
                                                    {
                                                        var BuyBtn = "<input type=\"button\" value=\"BuyNow\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"BuyQuoteBtn('" + oBOI.AttributeI("rfinalquote").sValue + "','" + sInstanceID + "',this " + ")\" />";
                                                        oBOIHTML = oBOIHTML.Replace(match.ToString(), BuyBtn);
                                                    }
                                                    else
                                                    {
                                                        string Code = match.ToString().TrimStart('{').TrimEnd('}');
                                                        var oLink = o1ClickC.MyLinks.Where(m => m.sCode == Code).FirstOrDefault();
                                                        //if (Code.StartsWith("Close"))
                                                        //{
                                                        //    if (oLink != null)
                                                        //    {
                                                        //        var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"CloseDetails('PolicyDetails_" + sInstanceID + "')\" />";
                                                        //        oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                        //    }
                                                        //}
                                                        if (Code.StartsWith("Make a Change"))
                                                        {
                                                            if (oLink != null)
                                                            {
                                                                var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaChange('" + sInstanceID + "')\" />";
                                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                            }
                                                        }
                                                        else if (Code.StartsWith("Make a Claim"))
                                                        {
                                                            if (oLink != null)
                                                            {
                                                                var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"MakeaClaim('" + sInstanceID + "')\" />";
                                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (oLink != null)
                                                            {
                                                                List<CNV> fncParams = new List<CNV>();//{XIP|ACPolicy_T.id}
                                                                fncParams.Add(new CNV { sName = oBOD.Name, sValue = sInstanceID });
                                                                fncParams.Add(new CNV { sName = "{XIP|" + oBOD.Name + ".id}", sValue = sInstanceID });
                                                                System.Web.Script.Serialization.JavaScriptSerializer oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                                string sJSON = oSerializer.Serialize(fncParams);
                                                                //var Btn = "<input type=\"button\" onclick=XILinkLoadJson(" + oLink.FKiXILinkID + ",'" + sGUID + "',JSON.parse('" + sJSON + "')) value=\"" + oLink.sName + "\" class=\"btn btn-theme\" data-id=" + sInstanceID + " />";
                                                                var Btn = "<input type=\"button\" value=\"" + oLink.sName + "\" class=\"btn btn-theme\"  data-id=" + sInstanceID + " onclick=\"fnc1clickcreate('" + o1ClickC.ID + "','" + o1ClickC.CreateGroupID + "','" + o1ClickC.BOID + "','" + oLink.FKiXILinkID + "','" + sInstanceID + "')\" />";
                                                                oBOIHTML = oBOIHTML.Replace(match.ToString(), Btn);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            o1ClickC.RepeaterResult.Add(oBOIHTML);
                                        }
                                    }
                                }
                                else if (o1ClickC.XIComponent.sName.ToLower() == XIConstant.FormComponent.ToLower())
                                {
                                    Dictionary<string, XIIBO> oDataSet = new Dictionary<string, XIIBO>();
                                    if (oRes.Count() == 0)
                                    {
                                        var oComp = o1ClickC.XIComponent;
                                        string sGroupName = oComp.Params.Where(m => m.sName.ToLower() == "bo").Select(m => m.sValue).FirstOrDefault();
                                        if (string.IsNullOrEmpty(sGroupName))
                                        {
                                            sGroupName = "create";
                                        }
                                        XIIBO oBOI = new XIIBO();
                                        oBOI.iBODID = o1ClickC.BOID;
                                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, o1ClickC.BOD.BOID.ToString());// o1ClickC.BOD;
                                        oBOI.LoadBOI(sGroupName);
                                        oBOI.BOD = null;
                                        oDataSet["0"] = oBOI;
                                        o1ClickC.oDataSet = oDataSet;
                                        o1ClickC.sIsFirstTime = "yes";
                                    }
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    if (o1ClickC.bIsCopy || o1ClickC.IsEdit || o1ClickC.bIsDelete || o1ClickC.bIsView)
                    //    {
                    //        o1ClickC.Headings.Add("Actions");
                    //        o1ClickC.TableColumns.Add("Actions");
                    //    }
                    //}
                    //Result = oXIRepo.GetHeadingsForList(OneClickID, null, sDatabase, 0, iUserID, sOrgName);
                    o1ClickC.sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sDisplayMode))
                    {
                        o1ClickC.ActionType = "View";
                    }
                }

                o1ClickC.BOD = null;
                oCResult.oResult = o1ClickC;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing 1-Click Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public string SubStringNoation(string HtmlContentTable, string sStartFormat, string sEndFormat)
        {
            try
            {
                string sFinalString = "";
                if (HtmlContentTable.Contains(sEndFormat))
                {
                    int iCollectionIndex = HtmlContentTable.IndexOf(sEndFormat);
                    int iStartPosi = HtmlContentTable.LastIndexOf(sStartFormat, iCollectionIndex);
                    int iStringLength = sEndFormat.Length;
                    sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                }
                return sFinalString;
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
    }
}